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
        BuildAndOpenLog(ScriptingImplementation.IL2CPP, BuildOptions.None, new[] { Build.PlatformWin32 }, "default.exe");
    }

    [MenuItem("Build/Standalone/build Windows 64bit IL2CPP (Slow)")]
    private static void Build_Win64()
    {
        BuildAndOpenLog(ScriptingImplementation.IL2CPP, BuildOptions.None, new[] { Build.PlatformWin64 }, "default.exe");
    }

    [MenuItem("Build/Standalone/build Windows 32bit Mono")]
    private static void Build_Win32_Mono()
    {
        BuildAndOpenLog(ScriptingImplementation.Mono2x, BuildOptions.None, new[] { Build.PlatformWin32 }, "default.exe");
    }

    [MenuItem("Build/Standalone/build Windows 64bit Mono")]
    private static void Build_Win64_Mono()
    {
        BuildAndOpenLog(ScriptingImplementation.Mono2x, BuildOptions.None, new[] { Build.PlatformWin64 }, "default.exe");
    }

    [MenuItem("Build/Standalone/build Mac Mono")]
    private static void Build_Mac()
    {
        BuildAndOpenLog(ScriptingImplementation.Mono2x, BuildOptions.None, new[] { Build.PlatformOsx }, "default.app");
    }

    [MenuItem("Build/Standalone/build All")]
    private static void Build_All()
    {
        BuildAndOpenLog(ScriptingImplementation.Mono2x, BuildOptions.None, new[] { Build.PlatformWin32, Build.PlatformWin64, Build.PlatformOsx }, "default.app");
    }

    [MenuItem("Build/Standalone/build Debug MNA Windows 64bit IL2CPP (Slow)", priority = 1100)]
    private static void Build_DebugWin64()
    {
        BuildAndOpenLog(ScriptingImplementation.IL2CPP, BuildOptions.Development | BuildOptions.AllowDebugging, new[] { Build.PlatformWin64 }, "default.exe");
    }

    [MenuItem("Build/Standalone/build Debug MNA (Scripts only) Windows 64bit IL2CPP (Slow)", priority = 1100)]
    private static void Build_DebugScriptsOnlyWin64()
    {
        BuildAndOpenLog(ScriptingImplementation.IL2CPP, BuildOptions.Development | BuildOptions.AllowDebugging | BuildOptions.BuildScriptsOnly, new[] { Build.PlatformWin64 }, "default.exe");
    }

    #endregion

    #region android
    
    [MenuItem("Build/Android/build android AAB (Slow)", priority = 1100)]
    private static void Build_Android_AAB()
    {
        BuildAndOpenLog(ScriptingImplementation.IL2CPP, BuildOptions.None, new[] { Build.PlatformAndroid }, "default.aab", true);
    }

    [MenuItem("Build/Android/build android IL2CPP (Slow)", priority = 1100)]
    private static void Build_Android_IL2CPP()
    {
        BuildAndOpenLog(ScriptingImplementation.IL2CPP, BuildOptions.None, new[] { Build.PlatformAndroid }, "default.apk");
    }

    [MenuItem("Build/Android/build android Mono", priority = 1100)]
    private static void Build_Android_Mono()
    {
        BuildAndOpenLog(ScriptingImplementation.Mono2x, BuildOptions.None, new[] { Build.PlatformAndroid }, "default.apk");
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
        BuildAndOpenLog(ScriptingImplementation.IL2CPP, BuildOptions.None, new[] { Build.PlatformWebGL }, "default");
    }

    [MenuItem("Build/WebGL/build WebGL Mono", priority = 1100)]
    private static void Build_WebGL_Mono()
    {
        BuildAndOpenLog(ScriptingImplementation.Mono2x, BuildOptions.None, new[] { Build.PlatformWebGL }, "default");
    }

    #endregion
    
    [MenuItem("Build/Addressable/Build (with schema rules)", priority = 1100)]
    private static void Build_Addressable_fresh()
    {
        AddressableBuildTool.BuildAddressable();
    }

    [MenuItem("Build/Addressable/Apply Schema Rules", priority = 1101)]
    private static void Apply_Schema_Rules()
    {
        AddressableBuildTool.ApplyConditionalBuildRules();
        Debug.Log("Schema-based conditional rules applied");
    }

    #region Test Conditional Builds

    [MenuItem("Build/Test Conditional/Simulate PRODUCTION Build", priority = 1200)]
    private static void Test_Production_Build()
    {
        Debug.Log("========== TESTING PRODUCTION BUILD ==========");
        
        // Temporarily set PRODUCTION define
        var target = EditorUserBuildSettings.selectedBuildTargetGroup;
        var originalDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);
        
        // Add PRODUCTION if not already present
        if (!originalDefines.Contains("PRODUCTION"))
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(target, originalDefines + ";PRODUCTION");
        }
        
        // Show current state
        Debug.Log($"Current Defines: {PlayerSettings.GetScriptingDefineSymbolsForGroup(target)}");
        Debug.Log($"Is PRODUCTION: {BuildTools.IsDefineSet("PRODUCTION")}");
        Debug.Log($"Is Development: {EditorUserBuildSettings.development}");
        
        // Apply rules and show results
        AddressableBuildTool.ApplyConditionalBuildRules();
        
        Debug.Log("========== TEST COMPLETE ==========");
        Debug.Log("Note: PRODUCTION define was added. Remove it via 'Clear Test Defines' if needed.");
    }

    [MenuItem("Build/Test Conditional/Simulate Development Build", priority = 1201)]
    private static void Test_Development_Build()
    {
        Debug.Log("========== TESTING DEVELOPMENT BUILD ==========");
        
        // Set development mode
        EditorUserBuildSettings.development = true;
        
        // Remove PRODUCTION define if present
        var target = EditorUserBuildSettings.selectedBuildTargetGroup;
        var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);
        defines = defines.Replace("PRODUCTION;", "").Replace(";PRODUCTION", "").Replace("PRODUCTION", "");
        PlayerSettings.SetScriptingDefineSymbolsForGroup(target, defines);
        
        // Show current state
        Debug.Log($"Current Defines: {PlayerSettings.GetScriptingDefineSymbolsForGroup(target)}");
        Debug.Log($"Is PRODUCTION: {BuildTools.IsDefineSet("PRODUCTION")}");
        Debug.Log($"Is Development: {EditorUserBuildSettings.development}");
        
        // Apply rules and show results
        AddressableBuildTool.ApplyConditionalBuildRules();
        
        Debug.Log("========== TEST COMPLETE ==========");
    }

    [MenuItem("Build/Test Conditional/Test With Creative Assets", priority = 1202)]
    private static void Test_With_Creative_Assets()
    {
        Debug.Log("========== TESTING WITH CREATIVE ASSETS ==========");
        
        var target = EditorUserBuildSettings.selectedBuildTargetGroup;
        var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);
        
        // Remove PRODUCTION and add INCLUDE_CREATIVE_ASSETS
        defines = defines.Replace("PRODUCTION;", "").Replace(";PRODUCTION", "").Replace("PRODUCTION", "");
        if (!defines.Contains("INCLUDE_CREATIVE_ASSETS"))
        {
            defines = string.IsNullOrEmpty(defines) ? "INCLUDE_CREATIVE_ASSETS" : defines + ";INCLUDE_CREATIVE_ASSETS";
        }
        
        PlayerSettings.SetScriptingDefineSymbolsForGroup(target, defines);
        
        // Show current state
        Debug.Log($"Current Defines: {PlayerSettings.GetScriptingDefineSymbolsForGroup(target)}");
        Debug.Log($"Include Creative: {BuildTools.IsDefineSet("INCLUDE_CREATIVE_ASSETS")}");
        
        // Apply rules and show results
        AddressableBuildTool.ApplyConditionalBuildRules();
        
        Debug.Log("========== TEST COMPLETE ==========");
    }

    [MenuItem("Build/Test Conditional/Show Current Symbol Status", priority = 1203)]
    private static void Show_Symbol_Status()
    {
        Debug.Log("========== CURRENT SYMBOL STATUS ==========");
        
        var target = EditorUserBuildSettings.selectedBuildTargetGroup;
        var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);
        
        Debug.Log($"Current Defines: {defines}");
        Debug.Log($"Build Target Group: {target}");
        Debug.Log($"Development Build: {EditorUserBuildSettings.development}");
        Debug.Log("");
        Debug.Log("Conditional Symbols:");
        Debug.Log($"  PRODUCTION: {BuildTools.IsDefineSet("PRODUCTION")}");
        Debug.Log($"  INCLUDE_DEBUG_ASSETS: {BuildTools.IsDefineSet("INCLUDE_DEBUG_ASSETS")}");
        Debug.Log($"  INCLUDE_CREATIVE_ASSETS: {BuildTools.IsDefineSet("INCLUDE_CREATIVE_ASSETS")}");
        Debug.Log($"  INCLUDE_EDITOR_ASSETS: {BuildTools.IsDefineSet("INCLUDE_EDITOR_ASSETS")}");
        Debug.Log("");
        Debug.Log("Note: Group inclusion is now determined by schema configurations on each group.");
        Debug.Log("Groups with IncludeInBuildWithSymbolSchema or ExcludeInBuildWithSymbolSchema");
        Debug.Log("will be included/excluded based on their configured symbols.");
        
        Debug.Log("========== END STATUS ==========");
    }

    [MenuItem("Build/Test Conditional/Clear All Test Defines", priority = 1204)]
    private static void Clear_Test_Defines()
    {
        Debug.Log("========== CLEARING TEST DEFINES ==========");
        
        var target = EditorUserBuildSettings.selectedBuildTargetGroup;
        var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);
        
        // Remove test-related defines
        var testDefines = new[] { "PRODUCTION", "INCLUDE_DEBUG_ASSETS", "INCLUDE_CREATIVE_ASSETS", "INCLUDE_EDITOR_ASSETS" };
        
        foreach (var define in testDefines)
        {
            defines = defines.Replace($"{define};", "").Replace($";{define}", "").Replace(define, "");
        }
        
        // Clean up any double semicolons or trailing semicolons
        defines = System.Text.RegularExpressions.Regex.Replace(defines, ";+", ";").Trim(';');
        
        PlayerSettings.SetScriptingDefineSymbolsForGroup(target, defines);
        EditorUserBuildSettings.development = false;
        
        Debug.Log($"Cleared defines. Current: {defines}");
        Debug.Log($"Development mode: {EditorUserBuildSettings.development}");
        
        Debug.Log("========== CLEAR COMPLETE ==========");
    }

    [MenuItem("Build/Test Conditional/Test Build And Log Groups", priority = 1205)]
    private static void Test_Build_And_Log_Groups()
    {
        Debug.Log("========== TESTING BUILD WITH GROUP LOGGING ==========");
        
        var settings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError("Addressables settings not found!");
            return;
        }
        
        // Log all groups and their current state
        Debug.Log($"Total Groups: {settings.groups.Count}");
        foreach (var group in settings.groups)
        {
            if (group == null) continue;
            
            var bundleSchema = group.GetSchema<UnityEditor.AddressableAssets.Settings.GroupSchemas.BundledAssetGroupSchema>();
            var includeSchema = group.GetSchema<IncludeInBuildWithSymbolSchema>();
            var excludeSchema = group.GetSchema<ExcludeInBuildWithSymbolSchema>();
            
            if (bundleSchema != null)
            {
                string schemaInfo = "";
                if (includeSchema != null) schemaInfo = " [Has IncludeSchema]";
                if (excludeSchema != null) schemaInfo = " [Has ExcludeSchema]";
                
                Debug.Log($"  {group.name}: IncludeInBuild = {bundleSchema.IncludeInBuild}{schemaInfo}");
            }
        }
        
        Debug.Log("");
        Debug.Log("Applying schema-based conditional rules...");
        AddressableBuildTool.ApplyConditionalBuildRules();
        
        Debug.Log("");
        Debug.Log("Groups after applying rules:");
        foreach (var group in settings.groups)
        {
            if (group == null) continue;
            
            var bundleSchema = group.GetSchema<UnityEditor.AddressableAssets.Settings.GroupSchemas.BundledAssetGroupSchema>();
            var includeSchema = group.GetSchema<IncludeInBuildWithSymbolSchema>();
            var excludeSchema = group.GetSchema<ExcludeInBuildWithSymbolSchema>();
            
            if (bundleSchema != null)
            {
                string schemaInfo = "";
                if (includeSchema != null) schemaInfo = " [Has IncludeSchema]";
                if (excludeSchema != null) schemaInfo = " [Has ExcludeSchema]";
                
                Debug.Log($"  {group.name}: IncludeInBuild = {bundleSchema.IncludeInBuild}{schemaInfo}");
            }
        }
        
        Debug.Log("========== TEST COMPLETE ==========");
    }

    #endregion



    [MenuItem("Build/Set scripting define symbols", priority = 100)]
    static void Build_SetScriptingDefineSymbols()
    {
        Build.SetScriptingDefineSymbolInternal(NamedBuildTarget.Android,
                                               "TextMeshPro;ODIN_INSPECTOR;ODIN_INSPECTOR_3;EASY_MOBILE;EASY_MOBILE_PRO;EM_ADMOB;EM_URP;ADDRESSABLES_ENABLED");
    }

    private static void BuildAndOpenLog(ScriptingImplementation scriptBackend, BuildOptions options, string[] platforms, string outputFile, bool buildAppBundle = false)
    {
        Build.BuildInternal(scriptBackend, options, platforms, outputFile, buildAppBundle);

        // Determine platform for log file
        var platform = platforms.Length == 1 ? platforms[0] : "multi";
        var logFileName = Build.GetLogFileName(platform);
        OpenLog(logFileName);
    }

    private static void OpenLog(string fileName)
    {
        if (!InternalEditorUtility.inBatchMode)
        {
            var d        = Directory.GetCurrentDirectory();
            var filePath = Path.GetFullPath($"{BuildScripts.Editor.BuildConstants.BUILD_LOGS_PATH}/{fileName}");
            if (File.Exists(filePath)) Process.Start(filePath);
        }
    }
}