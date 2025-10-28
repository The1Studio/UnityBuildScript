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

    /// <summary>
    /// Checks if a scripting define symbol is set for the currently selected build target group.
    /// </summary>
    /// <param name="define">The scripting define symbol to check (e.g., "PRODUCTION", "DEBUG").</param>
    /// <returns>True if the symbol is defined, false otherwise.</returns>
    public static bool IsDefineSet(string define)
    {
        return  IsDefineSet(define, EditorUserBuildSettings.selectedBuildTargetGroup);
    }

    /// <summary>
    /// Checks if a scripting define symbol is set for a specific build target group.
    /// Uses exact matching to avoid false positives (e.g., "PROD" won't match "PRODUCTION").
    /// </summary>
    /// <param name="define">The scripting define symbol to check.</param>
    /// <param name="targetGroup">The build target group to check (e.g., BuildTargetGroup.Android).</param>
    /// <returns>True if the symbol is defined for the target group, false otherwise.</returns>
    public static bool IsDefineSet(string define, BuildTargetGroup targetGroup)
    {
        PlayerSettings.GetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(targetGroup), out var defines);
        // Use exact match to avoid partial matches (e.g., "PROD" matching "PRODUCTION")
        return defines.Split(';').Any(d => d == define);
    }

    /// <summary>
    /// Removes a scripting define symbol from the specified build target group.
    /// </summary>
    /// <param name="targetGroup">The build target group to modify.</param>
    /// <param name="symbol">The symbol to remove.</param>
    public static void RemoveDefineSymbol(BuildTargetGroup targetGroup, string symbol)
    {
        var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
        defines = defines.Replace($"{symbol};", "")
                         .Replace($";{symbol}", "")
                         .Replace(symbol, "");
        defines = System.Text.RegularExpressions.Regex.Replace(defines, ";+", ";").Trim(';');
        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defines);
    }

    /// <summary>
    /// Adds a scripting define symbol to the specified build target group if it doesn't already exist.
    /// Uses exact matching to prevent duplicate additions.
    /// </summary>
    /// <param name="targetGroup">The build target group to modify.</param>
    /// <param name="symbol">The symbol to add.</param>
    public static void AddDefineSymbol(BuildTargetGroup targetGroup, string symbol)
    {
        var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
        // Use exact match to avoid partial matches
        if (!defines.Split(';').Any(d => d == symbol))
        {
            defines = string.IsNullOrEmpty(defines) ? symbol : defines + ";" + symbol;
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defines);
        }
    }

    /// <summary>
    /// Removes multiple scripting define symbols from the specified build target group.
    /// </summary>
    /// <param name="targetGroup">The build target group to modify.</param>
    /// <param name="symbols">The symbols to remove.</param>
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

    /// <summary>
    /// Checks if all specified scripting define symbols are set for the currently selected build target.
    /// Empty or whitespace-only symbols are ignored.
    /// </summary>
    /// <param name="symbols">The collection of symbols to check.</param>
    /// <returns>True if all non-empty symbols are defined, false if any are missing.</returns>
    public static bool AreAllDefinesSet(System.Collections.Generic.IEnumerable<string> symbols)
    {
        foreach (var symbol in symbols)
        {
            if (!string.IsNullOrWhiteSpace(symbol) && !IsDefineSet(symbol))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Checks if any of the specified scripting define symbols are set for the currently selected build target.
    /// Empty or whitespace-only symbols are ignored.
    /// </summary>
    /// <param name="symbols">The collection of symbols to check.</param>
    /// <returns>True if at least one non-empty symbol is defined, false if none are defined.</returns>
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