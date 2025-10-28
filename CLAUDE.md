# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview
This is The One Unity Build Script - a Unity Package Manager (UPM) package that provides automated build functionality for Unity projects across multiple platforms (Windows, Mac, Android, iOS, WebGL). Version 1.2.11, requires Unity 2022.3+.

**Package Structure:** This repository is a standalone UPM package (not a Unity project). Install it in Unity via Package Manager using Git URL or by adding to manifest.json.

## Critical Commands

### Building from Command Line
```bash
# Basic platform build
Unity -batchmode -quit -projectPath . -executeMethod Build.BuildFromCommandLine -platforms "win64"

# Android AAB with signing
Unity -batchmode -quit -projectPath . -executeMethod Build.BuildFromCommandLine \
  -platforms "android" \
  -buildAppBundle \
  -keyStoreFileName "path/to/keystore" \
  -keyStorePassword "password" \
  -keyStoreAliasName "alias" \
  -keyStoreAliasPassword "aliaspass" \
  -outputPath "builds/game.aab"

# iOS build with signing
Unity -batchmode -quit -projectPath . -executeMethod Build.BuildFromCommandLine \
  -platforms "ios" \
  -iosSigningTeamId "TEAMID" \
  -iosTargetOSVersion "12.0" \
  -outputPath "builds/ios"

# Development build with addressables
Unity -batchmode -quit -projectPath . -executeMethod Build.BuildFromCommandLine \
  -platforms "win64" \
  -development \
  -remoteAddressableBuildPath "ServerData/[BuildTarget]" \
  -remoteAddressableLoadPath "https://cdn.example.com/[BuildTarget]"
```

### Unity Editor Access
Menu items are under `Foundation/Build/`:
- Platform-specific builds (Win32/64, Mac, Android, iOS, WebGL)
- Build configurations (Mono/IL2CPP, Development/Production)
- Android keystore setup
- Addressables management

### Validation Commands
```bash
# Check for Unity compilation errors (requires Unity MCP)
mcp__UnityMCP__read_console

# Verify package structure
ls -la Editor/
```

## Architecture & Key Components

### Core Build System (`Editor/`)
- **Build.cs**: Main orchestrator with `BuildFromCommandLine()` entry point and platform configurations
- **BuildMenu.cs**: Unity Editor menu integration for GUI-based builds
- **BuildTools.cs**: Utility functions for build settings, assembly management, and scripting define symbols
- **BuildConstants.cs**: Centralized constants for paths, log files, GUI dimensions, and defaults (v1.2.11+)
- **AddressableBuildTool.cs**: Addressables with CDN support and compression modes
- **IOSPostProcessingBuildTool.cs**: Xcode project configuration and Swift settings

### Conditional Addressables (`Editor/Addressable/`)
- **ConditionalBuildSchemaBase.cs**: Abstract base class for conditional schema implementations (v1.2.11+)
- **IncludeInBuildWithSymbolSchema.cs**: Schema for including groups based on scripting define symbols
- **ExcludeInBuildWithSymbolSchema.cs**: Schema for excluding groups based on scripting define symbols

### Command-Line Arguments
- `-platforms`: Target platforms (win32/win64/osx/android/ios/webgl), semicolon-separated for multiple
- `-development`: Enable development build
- `-outputPath`: Build output location
- `-buildAppBundle`: Build Android AAB instead of APK
- `-optimizeSize`: Enable size optimization
- `-packageName`: Override application package name
- Android signing: `-keyStoreFileName`, `-keyStorePassword`, `-keyStoreAliasName`, `-keyStoreAliasPassword`
- iOS: `-iosTargetOSVersion`, `-iosSigningTeamId`
- Addressables: `-remoteAddressableBuildPath`, `-remoteAddressableLoadPath`

### Platform Configurations
Platform settings are defined in `Build.Targets` dictionary with BuildTargetInfo entries containing:
- Platform identifier and BuildTarget
- Output extension and subfolder
- Default build options
- Scripting backend support

### Build Flow
1. Parse arguments/menu selection → 2. Configure platform settings → 3. Set scripting defines → 4. Configure signing (Android/iOS) → 5. Build addressables → 6. Execute Unity build → 7. Post-process → 8. Generate report

## Development Guidelines

### Code Conventions
- **Naming**: PascalCase for classes/methods, camelCase for private fields
- **Organization**: Editor code in Editor folders, use assembly definitions
- **Platform code**: Use preprocessor directives (#if UNITY_ANDROID)
- **Menu items**: Use [MenuItem] with priority values for ordering
- **Constants**: Use BuildConstants for magic numbers, paths, and default values

### Architecture Patterns (v1.2.11+)
- **Inheritance**: Use abstract base classes to eliminate code duplication (see ConditionalBuildSchemaBase)
- **Constants**: Centralize magic numbers and paths in BuildConstants
- **Helper Methods**: Extract common logic into BuildTools helper methods
- **Single Responsibility**: Each class should have a focused purpose

### Making Changes
1. Always verify Unity compilation after changes using Unity Console or MCP tools
2. Test menu items appear correctly under Foundation/Build/
3. Update package.json version when adding features
4. Update CHANGELOG.md following Keep a Changelog format
5. Never commit sensitive data (keystores, passwords, API keys)
6. Test command-line builds after modifications

### Assembly Structure
- **BuildScript.Editor**: Editor-only build tools (references Addressables, ScriptableBuildPipeline)
- **BuildScript.Runtime**: Runtime components (minimal, for GameVersion access)
- Conditional PAD define for Play Asset Delivery when available

## Common Tasks

### Adding New Platform
1. Add platform entry to `Build.Targets` dictionary
2. Implement platform-specific configuration in `SpecificActionForEachPlatform()`
3. Add menu item in `BuildMenu.cs`
4. Update command-line parsing if new parameters needed

### Modifying Build Options
1. Edit BuildOptions in `BuildFromCommandLine()` or `BuildInternal()`
2. Add command-line argument parsing if needed
3. Update platform-specific options in `Targets` dictionary

### Adding New Constants
1. Add to `BuildConstants.cs` with appropriate category comment
2. Use throughout codebase instead of magic numbers
3. Update CHANGELOG.md documenting the constant addition

### Refactoring for Maintainability
1. Identify duplicate code patterns
2. Extract to base classes or helper methods
3. Use BuildConstants for any hardcoded values
4. Update all callers to use new abstraction
5. Document lines of code reduced in CHANGELOG.md

### Debugging Builds
1. Check build report via `Build.WriteReport()`
2. Use `-development` flag for detailed logging
3. Verify settings in Unity Build Settings window
4. Check Console for compilation errors before building
