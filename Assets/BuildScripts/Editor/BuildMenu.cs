using System.Diagnostics;
using System.IO;
using BuildScripts.Editor.Addressable;
using UnityEditor;
using UnityEditor.Build;
using UnityEditorInternal;
using Debug = UnityEngine.Debug;

public static class BuildMenu
{
    #region Standalone

    [MenuItem("Build/Standalone/build Windows 32bit IL2CPP (Slow)")]
    private static void Build_Win32()
    {
        Build.BuildInternal(ScriptingImplementation.IL2CPP, BuildOptions.None, new[] { Build.PlatformWin32 }, "default.exe");

        OpenLog("Build-Client-Report.win-x86.log");
    }

    [MenuItem("Build/Standalone/build Windows 64bit IL2CPP (Slow)")]
    private static void Build_Win64()
    {
        Build.BuildInternal(ScriptingImplementation.IL2CPP, BuildOptions.None, new[] { Build.PlatformWin64 }, "default.exe");

        OpenLog("Build-Client-Report.win-x64.log");
    }

    [MenuItem("Build/Standalone/build Windows 32bit Mono")]
    private static void Build_Win32_Mono()
    {
        Build.BuildInternal(ScriptingImplementation.Mono2x, BuildOptions.None, new[] { Build.PlatformWin32 }, "default.exe");

        OpenLog("Build-Client-Report.win-x86.log");
    }

    [MenuItem("Build/Standalone/build Windows 64bit Mono")]
    private static void Build_Win64_Mono()
    {
        Build.BuildInternal(ScriptingImplementation.Mono2x, BuildOptions.None, new[] { Build.PlatformWin64 }, "default.exe");

        OpenLog("Build-Client-Report.win-x64.log");
    }

    [MenuItem("Build/Standalone/build Mac Mono")]
    private static void Build_Mac()
    {
        Build.BuildInternal(ScriptingImplementation.Mono2x, BuildOptions.None, new[] { Build.PlatformOsx }, "default.app");

        OpenLog("Build-Client-Report.osx-x64.log");
    }

    [MenuItem("Build/Standalone/build All")]
    private static void Build_All() { Build.BuildInternal(ScriptingImplementation.Mono2x, BuildOptions.None, new[] { Build.PlatformWin32, Build.PlatformWin64, Build.PlatformOsx }, "default.app"); }

    [MenuItem("Build/Standalone/build Debug MNA Windows 64bit IL2CPP (Slow)", priority = 1100)]
    private static void Build_DebugWin64()
    {
        Build.BuildInternal(ScriptingImplementation.IL2CPP, BuildOptions.Development | BuildOptions.AllowDebugging, new[] { Build.PlatformWin64 }, "default.exe");

        OpenLog("Build-Client-Report.win-x64.log");
    }

    [MenuItem("Build/Standalone/build Debug MNA (Scripts only) Windows 64bit IL2CPP (Slow)", priority = 1100)]
    static void Build_DebugScriptsOnlyWin64()
    {
        Build.BuildInternal(ScriptingImplementation.IL2CPP, BuildOptions.Development | BuildOptions.AllowDebugging | BuildOptions.BuildScriptsOnly, new[] { Build.PlatformWin64 }, "default.exe");

        OpenLog("Build-Client-Report.win-x64.log");
    }

    #endregion

    #region android
    
    [MenuItem("Build/Android/build android AAB (Slow)", priority = 1100)]
    private static void Build_Android_AAB()
    {
        Build.BuildInternal(ScriptingImplementation.IL2CPP, BuildOptions.None, new[] { Build.PlatformAndroid }, "default.aab", true);

        OpenLog("Build-Client-Report.android.log");
    }

    [MenuItem("Build/Android/build android IL2CPP (Slow)", priority = 1100)]
    private static void Build_Android_IL2CPP()
    {
        Build.BuildInternal(ScriptingImplementation.IL2CPP, BuildOptions.None, new[] { Build.PlatformAndroid }, "default.apk");

        OpenLog("Build-Client-Report.android.log");
    }

    [MenuItem("Build/Android/build android Mono", priority = 1100)]
    private static void Build_Android_Mono()
    {
        Build.BuildInternal(ScriptingImplementation.Mono2x, BuildOptions.None, new[] { Build.PlatformAndroid }, "default.apk");

        OpenLog("Build-Client-Report.android.log");
    }

 
    [MenuItem("Build/Android/Setup keystore", priority = 1100)]
    private static void Build_Setup_KeyStore()
    {
        Build.SetUpAndroidKeyStore();
    }
    

    #endregion
    
    #region webgl

    [MenuItem("Build/WebGL/build WebGL IL2CPP (Slow)", priority = 1100)]
    private static void Build_WebGL_IL2CPP()
    {
        Build.BuildInternal(ScriptingImplementation.IL2CPP, BuildOptions.None, new[] { Build.PlatformWebGL }, "default");

        OpenLog("Build-Client-Report.webgl.log");
    }

    [MenuItem("Build/WebGL/build WebGL Mono", priority = 1100)]
    private static void Build_WebGL_Mono()
    {
        Build.BuildInternal(ScriptingImplementation.Mono2x, BuildOptions.None, new[] { Build.PlatformWebGL }, "default");

        OpenLog("Build-Client-Report.webgl.log");
    }

    #endregion
    
    [MenuItem("Build/Addressable/Build (with conditional rules)", priority = 1100)]
    private static void Build_Addressable_fresh()
    {
        AddressableBuildTool.BuildAddressable();
    }

    [MenuItem("Build/Addressable/Toggle Debug Groups", priority = 1101)]
    private static void Toggle_Debug_Groups()
    {
        bool isProduction = BuildTools.IsDefineSet("PRODUCTION");
        bool includeDebug = EditorUserBuildSettings.development || BuildTools.IsDefineSet("INCLUDE_DEBUG_ASSETS");
        
        if (isProduction)
        {
            includeDebug = false;
            Debug.Log("Debug groups forced to EXCLUDED (PRODUCTION mode)");
        }
        else
        {
            AddressableBuildTool.ToggleGroupsByNamePrefix("Debug", includeDebug);
            Debug.Log($"Debug groups set to: {(includeDebug ? "Included" : "Excluded")}");
        }
    }
 
    [MenuItem("Build/Addressable/Toggle Creative Groups", priority = 1102)]
    private static void Toggle_Creative_Groups()
    {
        bool isProduction = BuildTools.IsDefineSet("PRODUCTION");
        bool includeCreative = BuildTools.IsDefineSet("INCLUDE_CREATIVE_ASSETS");
        
        if (isProduction)
        {
            includeCreative = false;
            Debug.Log("Creative groups forced to EXCLUDED (PRODUCTION mode)");
        }
        else
        {
            AddressableBuildTool.ToggleGroupsByNamePrefix("Creative", includeCreative);
            Debug.Log($"Creative groups set to: {(includeCreative ? "Included" : "Excluded")}");
        }
    }

    [MenuItem("Build/Addressable/Apply Conditional Rules", priority = 1103)]
    private static void Apply_Conditional_Rules()
    {
        AddressableBuildTool.ApplyConditionalBuildRules();
        Debug.Log("Conditional rules applied based on current symbols");
    }



    [MenuItem("Build/Set scripting define symbols", priority = 100)]
    static void Build_SetScriptingDefineSymbols()
    {
        Build.SetScriptingDefineSymbolInternal(NamedBuildTarget.Android,
                                               "TextMeshPro;ODIN_INSPECTOR;ODIN_INSPECTOR_3;EASY_MOBILE;EASY_MOBILE_PRO;EM_ADMOB;EM_URP;ADDRESSABLES_ENABLED");
    }

    private static void OpenLog(string fileName)
    {
        if (!InternalEditorUtility.inBatchMode)
        {
            var d        = Directory.GetCurrentDirectory();
            var filePath = Path.GetFullPath($"../Build/Logs/{fileName}");
            if (File.Exists(filePath)) Process.Start(filePath);
        }
    }
}