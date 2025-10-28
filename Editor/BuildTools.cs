using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

public static class BuildTools
{
    [MenuItem("Build/Tools/List Player Assemblies in Console")]
    public static void PrintAssemblyNames()
    {
        Debug.Log("== Player Assemblies ==");
        Assembly[] playerAssemblies = CompilationPipeline.GetAssemblies(AssembliesType.Player);

        foreach (var assembly in playerAssemblies)
        {
            Debug.Log(assembly.name);
        }

        Debug.Log("== End ==");
    }

    [MenuItem("Build/Tools/List Assembly Definition Platform Names in Console")]
    public static void PrintAssemblyDefinitionPlatformNames()
    {
        Debug.Log("== Assembly Definition Platforms ==");
        var platforms = CompilationPipeline.GetAssemblyDefinitionPlatforms();

        foreach (var platform in platforms)
        {
            Debug.Log(
                $"DisplayName: {platform.DisplayName}, Name: {platform.Name}, BuildTarget: {platform.BuildTarget}"
            );
        }

        Debug.Log("== End ==");
    }

    [MenuItem("Build/Tools/Reset build settings")]
    public static void ResetBuildSettings()
    {
        EditorUserBuildSettings.allowDebugging = false;
        EditorUserBuildSettings.connectProfiler = false;
        EditorUserBuildSettings.buildScriptsOnly = false;
        EditorUserBuildSettings.buildWithDeepProfilingSupport = false;
        EditorUserBuildSettings.development = false;
        EditorUserBuildSettings.waitForManagedDebugger = false;
        EditorUserBuildSettings.waitForPlayerConnection = false;
    }

    [MenuItem("Build/Tools/Get editor user build settings")]
    public static void GetEditorUserBuildSettings()
    {
        var CopyPDBFiles = EditorUserBuildSettings.GetPlatformSettings("Standalone", "CopyPDBFiles");
        var CreateSolution = EditorUserBuildSettings.GetPlatformSettings("Standalone", "CreateSolution");

        var sb = new StringBuilder();
        sb.AppendLine("GetEditorUserBuildSettings");
        sb.AppendLine($"CopyPDBFiles = {CopyPDBFiles}");
        sb.AppendLine($"CreateSolution = {CreateSolution}");

        Debug.Log(sb.ToString());
    }

    public static bool IsDefineSet(string define)
    {
        return  IsDefineSet(define, EditorUserBuildSettings.selectedBuildTargetGroup);
    }

    public static bool IsDefineSet(string define, BuildTargetGroup targetGroup)
    {
        PlayerSettings.GetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(targetGroup), out var defines);
        return defines.Contains(define);
    }

    public static void RemoveDefineSymbol(BuildTargetGroup targetGroup, string symbol)
    {
        var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
        defines = defines.Replace($"{symbol};", "")
                         .Replace($";{symbol}", "")
                         .Replace(symbol, "");
        defines = System.Text.RegularExpressions.Regex.Replace(defines, ";+", ";").Trim(';');
        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defines);
    }

    public static void AddDefineSymbol(BuildTargetGroup targetGroup, string symbol)
    {
        var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
        if (!defines.Contains(symbol))
        {
            defines = string.IsNullOrEmpty(defines) ? symbol : defines + ";" + symbol;
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defines);
        }
    }

    public static void RemoveDefineSymbols(BuildTargetGroup targetGroup, params string[] symbols)
    {
        var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
        foreach (var symbol in symbols)
        {
            defines = defines.Replace($"{symbol};", "")
                             .Replace($";{symbol}", "")
                             .Replace(symbol, "");
        }
        defines = System.Text.RegularExpressions.Regex.Replace(defines, ";+", ";").Trim(';');
        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defines);
    }

    public static bool AreAllDefinesSet(System.Collections.Generic.IEnumerable<string> symbols)
    {
        foreach (var symbol in symbols)
        {
            if (!string.IsNullOrWhiteSpace(symbol) && !IsDefineSet(symbol))
                return false;
        }
        return true;
    }

    public static bool IsAnyDefineSet(System.Collections.Generic.IEnumerable<string> symbols)
    {
        foreach (var symbol in symbols)
        {
            if (!string.IsNullOrWhiteSpace(symbol) && IsDefineSet(symbol))
                return true;
        }
        return false;
    }
}