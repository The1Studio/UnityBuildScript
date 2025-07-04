using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BuildScripts.Editor.Addressable;
using GameFoundation.BuildScripts.Runtime;
using Unity.CodeEditor;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Compilation;
#if UNITY_WEBGL
using UnityEditor.WebGL;
#else
using UnityEditor.Android;
#endif
using UnityEngine;
#if UNITY_6000_0_OR_NEWER
using Unity.Android.Types;
using AndroidApplicationEntry = UnityEditor.AndroidApplicationEntry;
using AndroidArchitecture = UnityEditor.AndroidArchitecture;
#endif

// ------------------------------------------------------------------------
// https://docs.unity3d.com/Manual/CommandLineArguments.html
// ------------------------------------------------------------------------
public static class Build
{
    public const string PlatformOsx     = "osx-x64";
    public const string PlatformWin64   = "win-x64";
    public const string PlatformWin32   = "win-x86";
    public const string PlatformAndroid = "android";
    public const string PlatformIOS     = "ios";
    public const string PlatformWebGL   = "webgl";

    private class BuildTargetInfo
    {
        public string           Platform; // eg "win-x64"
        public BuildTarget      BuildTarget;
        public BuildTargetGroup BuildTargetGroup;
        public NamedBuildTarget NamedBuildTarget;
    }

    private static readonly List<BuildTargetInfo> Targets = new()
    {
        new BuildTargetInfo
        {
            Platform         = PlatformWin32, BuildTarget = BuildTarget.StandaloneWindows,
            BuildTargetGroup = BuildTargetGroup.Standalone,
            NamedBuildTarget = NamedBuildTarget.Standalone
        },
        new BuildTargetInfo
        {
            Platform         = PlatformWin64, BuildTarget = BuildTarget.StandaloneWindows64,
            BuildTargetGroup = BuildTargetGroup.Standalone,
            NamedBuildTarget = NamedBuildTarget.Standalone
        },
        new BuildTargetInfo
        {
            Platform         = PlatformOsx, BuildTarget = BuildTarget.StandaloneOSX,
            BuildTargetGroup = BuildTargetGroup.Standalone,
            NamedBuildTarget = NamedBuildTarget.Standalone
        },
        new BuildTargetInfo
        {
            Platform         = PlatformAndroid, BuildTarget = BuildTarget.Android, BuildTargetGroup = BuildTargetGroup.Android,
            NamedBuildTarget = NamedBuildTarget.Android
        },
        new BuildTargetInfo
        {
            Platform         = PlatformIOS, BuildTarget = BuildTarget.iOS, BuildTargetGroup = BuildTargetGroup.iOS,
            NamedBuildTarget = NamedBuildTarget.iOS
        },
        new BuildTargetInfo
        {
            Platform         = PlatformWebGL, BuildTarget = BuildTarget.WebGL, BuildTargetGroup = BuildTargetGroup.WebGL,
            NamedBuildTarget = NamedBuildTarget.WebGL
        }
    };

    private static string[] SCENES                = FindEnabledEditorScenes();
    private static bool     OptimizeBuildSie      = false;
    private static string   keyStoreFileName      = "the1_googleplay.keystore";
    private static string   keyStoreAliasName     = "theonestudio";
    private static string   keyStorePassword      = "tothemoon";
    private static string   keyStoreAliasPassword = "tothemoon";
    private static string   iosTargetOSVersion    = "13.0";
    private static string   iosSigningTeamId      = "";


    private static BuildTargetInfo[] GetBuildTargetInfoFromString(string platforms)
    {
        return platforms.Split(';').Select(platformText => Targets.Single(t => t.Platform == platformText))
            .ToArray();
    }

    private static BuildTargetInfo[] GetBuildTargetInfoFromString(IEnumerable<string> platforms)
    {
        return platforms.Select(platformText => Targets.Single(t => t.Platform == platformText))
            .ToArray();
    }

    public static void SetScriptingDefineSymbols()
    {
        var args                   = Environment.GetCommandLineArgs();
        var platforms              = string.Join(";", Targets.Select(t => t.Platform));
        var scriptingDefineSymbols = string.Empty;

        for (var i = 0; i < args.Length; ++i)
        {
            switch (args[i])
            {
                case "-platforms":
                    platforms = args[++i];
                    break;
                case "-scriptingDefineSymbols":
                    scriptingDefineSymbols = args[++i];
                    break;
            }
        }

        if (string.IsNullOrEmpty(scriptingDefineSymbols)) return;

        foreach (var buildTargetInfo in GetBuildTargetInfoFromString(platforms))
        {
            SetScriptingDefineSymbolInternal(buildTargetInfo.NamedBuildTarget, scriptingDefineSymbols);
        }
    }

    public static void BuildFromCommandLine()
    {
#if UNITY_ANDROID
        AndroidExternalToolsSettings.gradlePath = null;
#endif
        // Grab the CSV platforms string
        var platforms = string.Join(";", Targets.Select(t => t.Platform));
        var args      = Environment.GetCommandLineArgs();
#if PRODUCTION
        var buildOptions = BuildOptions.CompressWithLz4HC | BuildOptions.CleanBuildCache;
#else
        var buildOptions = BuildOptions.CompressWithLz4; // Increase build time
#endif
        var outputPath                 = "template.exe";
        var buildAppBundle             = false;
        var packageName                = "";
        var remoteAddressableBuildPath = "";
        var remoteAddressableLoadPath  = "";

        PlayerSettings.Android.useCustomKeystore = false;
        for (var i = 0; i < args.Length; ++i)
        {
            switch (args[i])
            {
                case "-platforms":
                    platforms = args[++i];
                    break;
                case "-development":
                    buildOptions |= BuildOptions.Development;
                    break;
                case "-outputPath":
                    outputPath = args[++i];
                    break;
                case "-buildAppBundle":
                    buildAppBundle = true;
                    break;
                case "-optimizeSize":
                    OptimizeBuildSie = true;
                    break;
                case "-packageName":
                    packageName = args[++i];
                    break;
                case "-keyStoreFileName":
                    keyStoreFileName = args[++i];
                    break;
                case "-keyStorePassword":
                    keyStorePassword = args[++i];
                    break;
                case "-keyStoreAliasName":
                    keyStoreAliasName = args[++i];
                    break;
                case "-keyStoreAliasPassword":
                    keyStoreAliasPassword = args[++i];
                    break;
                case "-iosTargetOSVersion":
                    iosTargetOSVersion = args[++i];
                    break;
                case "-iosSigningTeamId":
                    iosSigningTeamId = args[++i];
                    break;
                case "-remoteAddressableBuildPath":
                    remoteAddressableBuildPath = args[++i];
                    break;
                case "-remoteAddressableLoadPath":
                    remoteAddressableLoadPath = args[++i];
                    break;
            }
        }

        PlayerSettings.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
#if PRODUCTION
            PlayerSettings.SetStackTraceLogType(LogType.Assert,  StackTraceLogType.None);
            PlayerSettings.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
#endif
        if (!string.IsNullOrEmpty(remoteAddressableBuildPath) && !string.IsNullOrEmpty(remoteAddressableLoadPath))
        {
            AddressableBuildTool.CreateOrUpdateTheOneCDNProfile(remoteAddressableBuildPath, remoteAddressableLoadPath);
        }

        PlayerSettings.SplashScreen.showUnityLogo = false;
        // Get a list of targets to build
#if !PR_CHECK
        var scriptBackend = ScriptingImplementation.IL2CPP;
#else
        var scriptBackend = ScriptingImplementation.Mono2x;
#endif
        BuildInternal(scriptBackend, buildOptions, platforms.Split(";"), outputPath,
            buildAppBundle, packageName);
    }

    private static void SetupIos(string teamId, string targetOSVersion)
    {
        PlayerSettings.iOS.appleDeveloperTeamID  = teamId;
        PlayerSettings.iOS.targetOSVersionString = targetOSVersion;
    }

    public static void SetUpAndroidKeyStore(string keyStoreFileName = "the1_googleplay.keystore",
        string keyStorePass = "tothemoon", string keyaliasName = "theonestudio",
        string keyaliasPass = "tothemoon")
    {
        Console.WriteLine("-----Setup android keystore-----");
        Console.WriteLine($"keystore file name: {keyStoreFileName}");
        Console.WriteLine($"keystore file pass: {keyStorePass}");
        Console.WriteLine($"keystore alias name: {keyaliasName}");
        Console.WriteLine($"keystore alias pass: {keyaliasPass}");

        PlayerSettings.Android.useCustomKeystore = true;
        PlayerSettings.Android.keystoreName      = keyStoreFileName;
        PlayerSettings.Android.keystorePass      = keyStorePass;
        PlayerSettings.Android.keyaliasName      = keyaliasName;
        PlayerSettings.Android.keyaliasPass      = keyaliasPass;
        Console.WriteLine("-----Setup android keystore finished-----");
    }

    public static void BuildInternal(ScriptingImplementation scriptBackend, BuildOptions options,
        IEnumerable<string> platforms, string outputPath,
        bool buildAppBundle = false, string packageName = "")
    {
#if DEEP_PROFILING
            options |= BuildOptions.EnableDeepProfilingSupport;
#endif
        BuildTools.ResetBuildSettings();

        var buildTargetInfos = GetBuildTargetInfoFromString(platforms);
        Console.WriteLine("Building Targets: " +
                          string.Join(", ",
                              buildTargetInfos.Select(target => target.Platform)
                                  .ToArray())); // Log which targets we're gonna build

        var errors = false;
        foreach (var platform in buildTargetInfos)
        {
            Console.WriteLine($"----------{new string('-', platform.Platform.Length)}");
            Console.WriteLine($"Building: {platform.Platform}");
            Console.WriteLine($"----------{new string('-', platform.Platform.Length)}");

            if (!string.IsNullOrEmpty(packageName))
            {
                PlayerSettings.SetApplicationIdentifier(platform.NamedBuildTarget, packageName);
            }

            if (EditorUserBuildSettings.activeBuildTarget != platform.BuildTarget)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(platform.BuildTargetGroup, platform.BuildTarget);
            }

            SpecificActionForEachPlatform(platform);
            SetApplicationVersion();
            PlayerSettings.SetScriptingBackend(platform.NamedBuildTarget, scriptBackend);

#if UNITY_IOS
        SetupIos(iosSigningTeamId, iosTargetOSVersion);
#endif // UNITY_IOS

#if UNITY_ANDROID
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
#if UNITY_6000_1_OR_NEWER
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel36;
#else
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel35;
#endif
            EditorUserBuildSettings.buildAppBundle = buildAppBundle;

            if (buildAppBundle)
            {
                SetUpAndroidKeyStore(keyStoreFileName, keyStorePassword, keyStoreAliasName, keyStoreAliasPassword);
#if UNITY_6000_0_OR_NEWER
                UserBuildSettings.DebugSymbols.level = DebugSymbolLevel.Full;
#else
            EditorUserBuildSettings.androidCreateSymbols = AndroidCreateSymbols.Debugging;
#endif
            }
            else
            {
#if UNITY_6000_0_OR_NEWER
                UserBuildSettings.DebugSymbols.level = DebugSymbolLevel.None;
#else
            EditorUserBuildSettings.androidCreateSymbols = AndroidCreateSymbols.Disabled;
#endif
            }
#endif //UNITY_ANDROID

            AddressableBuildTool.BuildAddressable();

            // Set up the build options
            if (platform.Platform.Equals(PlatformWebGL))
                options &= ~BuildOptions
                    .Development; // can't build development for webgl, it make the build larger and cant gzip
            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes           = SCENES,
                locationPathName = Path.GetFullPath($"../Build/Client/{platform.Platform}/{outputPath}"),
                target           = platform.BuildTarget,
                options          = options
            };

            // Perform the build
            var buildResult = BuildPipeline.BuildPlayer(buildPlayerOptions);
            WriteReport(buildResult);
            errors = errors || buildResult.summary.result != BuildResult.Succeeded;
        }

        Console.WriteLine(errors ? "*** Some targets failed ***" : "All targets built successfully!");

        Console.WriteLine(new string('=', 80));
        Console.WriteLine();
    }

    private static void SpecificActionForEachPlatform(BuildTargetInfo platform)
    {
#if !UNITY_2022_1_OR_NEWER
        EditorUserBuildSettings.il2CppCodeGeneration = il2CppCodeGeneration;
#endif
#if PRODUCTION
        PlayerSettings.SetManagedStrippingLevel(platform.NamedBuildTarget, ManagedStrippingLevel.High);
        PlayerSettings.stripEngineCode = true;
#else
        PlayerSettings.SetManagedStrippingLevel(platform.NamedBuildTarget, ManagedStrippingLevel.Minimal);
        PlayerSettings.stripEngineCode = false;
#endif
        switch (platform.BuildTarget)
        {
            case BuildTarget.iOS:
                PlayerSettings.iOS.appleEnableAutomaticSigning = true;
                break;
            case BuildTarget.Android:
                //Change build architecture to ARMv7 and ARM6
#if !UNITY_2022_1_OR_NEWER
                PlayerSettings.Android.minifyWithR8 = true;
#endif
#if UNITY_6000_0_OR_NEWER
                PlayerSettings.Android.optimizedFramePacing = false;
#if !GAME_ACTIVITY
                PlayerSettings.Android.applicationEntry = AndroidApplicationEntry.Activity;
#else
                PlayerSettings.Android.applicationEntry = AndroidApplicationEntry.GameActivity;
#endif
#endif
                PlayerSettings.Android.minifyRelease       = true;
                PlayerSettings.Android.minifyDebug         = true;
                PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
#if UNITY_2022_1_OR_NEWER
                var il2CppCodeGeneration = OptimizeBuildSie ? Il2CppCodeGeneration.OptimizeSize : Il2CppCodeGeneration.OptimizeSpeed;
                PlayerSettings.SetIl2CppCodeGeneration(NamedBuildTarget.Android, il2CppCodeGeneration);
#endif
                break;
#if UNITY_WEBGL
            case BuildTarget.WebGL:
                PlayerSettings.SetManagedStrippingLevel(platform.NamedBuildTarget, ManagedStrippingLevel.High);
#if WEBGL_BROTLI
                PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Brotli;
                PlayerSettings.WebGL.decompressionFallback = true;
#elif WEBGL_GZIP
                PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
                PlayerSettings.WebGL.decompressionFallback = true;
#else
                PlayerSettings.WebGL.compressionFormat     = WebGLCompressionFormat.Disabled;
                PlayerSettings.WebGL.decompressionFallback = false;
#endif
                PlayerSettings.runInBackground         = false;
                PlayerSettings.WebGL.powerPreference   = WebGLPowerPreference.Default;
                PlayerSettings.WebGL.dataCaching       = true;
                PlayerSettings.WebGL.nameFilesAsHashes = true;
#if PRODUCTION
                PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.ExplicitlyThrownExceptionsOnly;
#else
                PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.FullWithStacktrace;
#endif
#if UNITY_2022_1_OR_NEWER
                PlayerSettings.WebGL.initialMemorySize = 64;
                UserBuildSettings.codeOptimization     = WasmCodeOptimization.DiskSizeLTO;
                PlayerSettings.SetIl2CppCodeGeneration(NamedBuildTarget.WebGL, Il2CppCodeGeneration.OptimizeSize);
                PlayerSettings.WebGL.showDiagnostics = false;
#if FB_INSTANT || PRODUCTION
                PlayerSettings.WebGL.showDiagnostics = false;
#else
                PlayerSettings.WebGL.showDiagnostics = true;
#endif // FB_INSTANT

#endif // UNITY_2022_1_OR_NEWER
                break;
#endif //UNITY_WEBGL
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// Find all active sences in build setting.
    /// </summary>
    /// <returns></returns>
    private static string[] FindEnabledEditorScenes() =>
        (from scene in EditorBuildSettings.scenes where scene.enabled select scene.path).ToArray();

    private static void WriteReport(BuildReport report)
    {
        Directory.CreateDirectory("../Build/Logs");
        var platform = Targets.SingleOrDefault(t => t.BuildTarget == report.summary.platform)?.Platform ?? "unknown";
        var filePath = $"../Build/Logs/Build-Client-Report.{platform}.log";
        var summary  = report.summary;
        using (var file = new StreamWriter(filePath))
        {
            file.WriteLine($"Build {summary.guid} for {summary.platform}.");
            file.WriteLine(
                $"Build began at {summary.buildStartedAt} and ended at {summary.buildEndedAt}. Total {summary.totalTime}.");
            file.WriteLine($"Build options: {summary.options}");
            file.WriteLine($"Build output to: {summary.outputPath}");
            file.WriteLine(
                $"Build result: {summary.result} ({summary.totalWarnings} warnings, {summary.totalErrors} errors).");
            file.WriteLine($"Build size: {summary.totalSize}");

            file.WriteLine();

            foreach (var step in report.steps)
            {
                WriteStep(file, step);
            }

            file.WriteLine();

#if UNITY_2022_1_OR_NEWER
            foreach (var buildFile in report.GetFiles())
#else
            foreach (var buildFile in report.files)
#endif
            {
                file.WriteLine($"Role: {buildFile.role}, Size: {buildFile.size} bytes, Path: {buildFile.path}");
            }

            file.WriteLine();
        }
    }

    private static void WriteStep(StreamWriter file, BuildStep step)
    {
        file.WriteLine($"Step {step.name}  Depth: {step.depth} Time: {step.duration}");
        foreach (var message in step.messages)
        {
            file.WriteLine($"{Prefix(message.type)}: {message.content}");
        }

        file.WriteLine();
    }

    private static string Prefix(LogType type) =>
        type switch
        {
            LogType.Assert => "A",
            LogType.Error => "E",
            LogType.Exception => "X",
            LogType.Log => "L",
            LogType.Warning => "W",
            _ => "????"
        };

    /// <summary>
    ///  Sync build version with blockchain server, photon server by version file which was generated by jenkins
    /// </summary>
    private static void SetApplicationVersion()
    {
        // Bundle version will be use for some third party like Backtrace, DeltaDNA,...
        PlayerSettings.bundleVersion = GameVersion.Version;
#if UNITY_ANDROID
        PlayerSettings.Android.bundleVersionCode = GameVersion.BuildNumber;
#elif UNITY_IOS
        PlayerSettings.iOS.buildNumber = GameVersion.BuildNumber.ToString();
#endif
    }

    public static void SetScriptingDefineSymbolInternal(NamedBuildTarget namedBuildTarget,
        string scriptingDefineSymbols)
    {
        PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, scriptingDefineSymbols);
        CompilationPipeline.RequestScriptCompilation();
        CodeEditor.Editor.CurrentCodeEditor.SyncAll();
    }
}