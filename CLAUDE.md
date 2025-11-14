# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview
Unity Package Manager (UPM) package for automated multi-platform builds. Current version: 1.2.19, requires Unity 2022.3+.

**Critical:** This is a UPM package repository, NOT a Unity project. No Assets/ or ProjectSettings/ folders. All Editor code lives in `Editor/`, runtime code in `Runtime/`.

## Build Commands

### Command-Line Builds
Entry points (use both for full build):
- `Build.BuildAddressablesOnly()` - Build addressables only
- `Build.BuildFromCommandLine()` - Build player only (always skips addressables)

**Important:** Full build = BuildAddressablesOnly() + BuildFromCommandLine()

```bash
# Player build only (addressables NOT included)
Unity -batchmode -quit -projectPath <unity-project> -executeMethod Build.BuildFromCommandLine -platforms "win64"

# Addressables build only (no player)
Unity -batchmode -quit -projectPath <unity-project> -executeMethod Build.BuildAddressablesOnly

# Addressables with remote CDN configuration
Unity -batchmode -quit -projectPath <unity-project> -executeMethod Build.BuildAddressablesOnly \
  -remoteAddressableBuildPath "ServerData/[BuildTarget]" \
  -remoteAddressableLoadPath "https://cdn.example.com/[BuildTarget]"

# FULL BUILD: Addressables + Player
Unity -batchmode -quit -projectPath <unity-project> -executeMethod Build.BuildAddressablesOnly
Unity -batchmode -quit -projectPath <unity-project> -executeMethod Build.BuildFromCommandLine -platforms "win64"

# Multiple platforms (player only, addressables separate)
Unity -batchmode -quit -projectPath <unity-project> -executeMethod Build.BuildFromCommandLine -platforms "win64;android;ios"

# Android AAB with signing
Unity -batchmode -quit -projectPath <unity-project> -executeMethod Build.BuildFromCommandLine \
  -platforms "android" \
  -buildAppBundle \
  -keyStoreFileName "path/to/keystore" \
  -keyStorePassword "password" \
  -keyStoreAliasName "alias" \
  -keyStoreAliasPassword "aliaspass"

# Development build with remote addressables
Unity -batchmode -quit -projectPath <unity-project> -executeMethod Build.BuildFromCommandLine \
  -platforms "win64" \
  -development \
  -remoteAddressableBuildPath "ServerData/[BuildTarget]" \
  -remoteAddressableLoadPath "https://cdn.example.com/[BuildTarget]"
```

**Key Arguments:**
- `-platforms`: win32, win64, osx, android, ios, webgl (semicolon-separated for multiple)
- `-development`: Enable development build
- `-outputPath`: Custom build output directory
- `-buildAppBundle`: Android AAB instead of APK
- `-optimizeSize`: Size optimization
- `-packageName`: Override app package name
- Android: `-keyStoreFileName`, `-keyStorePassword`, `-keyStoreAliasName`, `-keyStoreAliasPassword`
- iOS: `-iosTargetOSVersion`, `-iosSigningTeamId`
- Addressables: `-remoteAddressableBuildPath`, `-remoteAddressableLoadPath`

**Note:** BuildFromCommandLine always skips addressables. Build them separately with BuildAddressablesOnly.

### Unity Editor Menu
Menu items under `Build/` (defined in Editor/BuildMenu.cs):
- **Build/Standalone/** - Win32/64, Mac builds with Mono/IL2CPP options
- **Build/Android/** - Android APK/AAB builds, keystore setup
- **Build/WebGL/** - WebGL builds
- **Build/Addressable/Build (with schema rules)** - Build addressables only
- **Build/Addressable/Apply Schema Rules** - Apply conditional rules without building
- **Build/Test Conditional/** - Test conditional addressable group configurations

### Running Tests
```bash
# Run tests via Unity Test Framework
Unity -batchmode -runTests -projectPath <unity-project> -testPlatform EditMode -testResults results.xml
```

Test files in `Tests/Editor/`:
- BuildTests.cs - Platform constants validation
- BuildToolsTests.cs - Scripting define symbols utilities
- ConditionalSchemaTests.cs - Addressable conditional schemas
- BuildConstantsTests.cs - Constants validation

### Validation
```bash
# Check Unity compilation (requires Unity MCP server)
mcp__UnityMCP__read_console
```

## Architecture

### Build Flow

**Player Build (BuildFromCommandLine):**
Parse arguments → Configure platform (Build.cs) → Set scripting defines (BuildTools.cs) → Configure signing (Android/iOS) → Execute Unity build (AddressableBuildTool skipped) → Post-process (IOSPostProcessingBuildTool.cs) → Generate report

**Addressables Build (BuildAddressablesOnly):**
Parse arguments → Configure CDN profile (if remote paths provided) → Build addressables (AddressableBuildTool.cs)

**Full Build = BuildAddressablesOnly() + BuildFromCommandLine()**

These are completely separate entry points. Call both in sequence for a complete build.

### Core Components

**Build.cs** - Main orchestrator
- `BuildFromCommandLine()`: Entry point for player builds (always skips addressables)
- `BuildAddressablesOnly()`: Entry point for addressables-only builds
- `BuildInternal()`: Core build logic with `skipAddressables=true` hardcoded
- `Targets` dictionary: Platform configurations (BuildTargetInfo with platform ID, BuildTarget, extensions, options)
- Platform constants: PlatformWin32, PlatformWin64, PlatformOsx, PlatformAndroid, PlatformIOS, PlatformWebGL
- Parse command-line args and coordinate build process

**Two separate build entry points:**
1. BuildAddressablesOnly() - builds addressables
2. BuildFromCommandLine() - builds player
3. Full build = call both

**BuildMenu.cs** - Editor GUI integration
- [MenuItem] attributes for Foundation/Build/ menu
- Calls Build.cs methods with appropriate arguments
- Uses `BuildAndOpenLog()` helper to reduce duplication (v1.2.11+)

**BuildTools.cs** - Shared utilities
- `IsDefineSet()`, `AreAllDefinesSet()`, `IsAnyDefineSet()`: Check scripting define symbols
- `AddDefineSymbol()`, `RemoveDefineSymbol()`, `RemoveDefineSymbols()`: Manage defines
- Assembly listing and build settings reset tools

**BuildConstants.cs** - Centralized constants (v1.2.11+)
- Log file paths, default values, GUI dimensions
- Use this for all magic numbers instead of hardcoding

**Editor/Addressable/AddressableBuildTool.cs** - Addressables builder
- Remote catalog configuration
- CDN path setup
- Compression modes (LZ4 for WebGL, LZMA for production)
- Conditional group inclusion/exclusion based on schemas

**Editor/Addressable/ConditionalBuildSchemaBase.cs** - Schema base class (v1.2.11+)
- Abstract base for conditional addressable group schemas
- Provides common symbol list UI, validation, and logic checking
- Eliminates ~150-180 lines of duplication

**Editor/Addressable/IncludeInBuildWithSymbolSchema.cs**
- Attach to addressable groups to include only when specific scripting defines present
- "Require All" or "Any" logic modes
- Extends ConditionalBuildSchemaBase (56 lines, down from 113)

**Editor/Addressable/ExcludeInBuildWithSymbolSchema.cs**
- Attach to addressable groups to exclude when specific scripting defines present
- "Exclude If Any" or "All" logic modes
- Extends ConditionalBuildSchemaBase (62 lines, down from 118)

**Editor/IOSPostProcessingBuildTool.cs**
- Xcode project configuration post-build
- Swift support and framework linking

**Runtime/GameVersion.cs**
- Minimal runtime component for build version access

### Assembly Definitions
- **BuildScript.Editor.asmdef**: References Addressables.Editor, ScriptableBuildPipeline.Editor, UniTask
- **BuildScript.Runtime.asmdef**: Minimal runtime, no dependencies
- **BuildScript.Editor.Tests.asmdef**: NUnit test assembly
- Conditional `PAD` define when com.unity.addressables.android available

## Development Patterns

### Code Conventions
- PascalCase for public members, camelCase for private fields
- Editor-only code in Editor/ folders with Editor assembly definitions
- Platform-specific code: `#if UNITY_ANDROID`, `#if UNITY_IOS`, `#if UNITY_WEBGL`
- Menu items: `[MenuItem("Foundation/Build/...")]` with priority values for ordering
- Always use BuildConstants for paths, defaults, and magic numbers

### Architecture Principles (v1.2.11+)
- **DRY via inheritance**: Extract common patterns to base classes (see ConditionalBuildSchemaBase)
- **Centralized constants**: All magic numbers → BuildConstants.cs
- **Helper methods**: Common logic → BuildTools static methods
- **Single responsibility**: Each class has one focused purpose

### Making Changes Checklist
1. Read entire file before editing to understand context and avoid duplicating existing code
2. Verify Unity compilation: `mcp__UnityMCP__read_console` or check Unity Console
3. Test Editor menu items appear under Foundation/Build/
4. Run tests: Unity Test Framework in Editor mode
5. Update package.json version for new features (semantic versioning)
6. Update CHANGELOG.md following Keep a Changelog format (Added/Changed/Fixed/Removed sections)
7. NEVER commit credentials (keystores, passwords, API keys)
8. Test command-line build for modified platforms

### Refactoring Pattern (v1.2.11+ standard)
When adding features that involve duplication:
1. Identify common patterns across classes
2. Extract to abstract base class or helper methods
3. Replace magic numbers with BuildConstants entries
4. Update all implementations to use new abstractions
5. Document LOC reduction in CHANGELOG.md

Example: ConditionalBuildSchemaBase reduced IncludeInBuildWithSymbolSchema (113→56 lines) and ExcludeInBuildWithSymbolSchema (118→62 lines), eliminating ~150-180 duplicate lines.

## Common Development Tasks

### Adding New Platform
1. Add entry to `Build.Targets` list with BuildTargetInfo (platform string, BuildTarget enum, extension, options)
2. Add platform constant: `public const string PlatformX = "platform-id";`
3. Implement platform-specific logic in `SpecificActionForEachPlatform()` if needed
4. Add menu items in BuildMenu.cs with [MenuItem] attributes
5. Update command-line parsing in `BuildFromCommandLine()` for new arguments
6. Add tests in BuildTests.cs for new platform constant

### Adding Command-Line Arguments
1. Parse in `Build.BuildFromCommandLine()` using `GetArg()` helper
2. Store in static field or pass to build methods
3. Apply in `BuildInternal()` or platform-specific methods
4. Document in README.md arguments table
5. Test via command-line build

### Modifying Addressables Behavior
1. Edit `AddressableBuildTool.cs` for build-time logic
2. Use `ConditionalBuildSchemaBase` if creating new conditional schemas
3. Test with remote/local builds and multiple platforms
4. Verify compression modes (LZ4 for WebGL, LZMA for others)

### Adding Constants
1. Add to `BuildConstants.cs` in appropriate category section
2. Replace all hardcoded instances throughout codebase
3. Update CHANGELOG.md noting the addition
4. Add test in BuildConstantsTests.cs to verify value

### Debugging Build Failures
1. Check Unity Console compilation errors before building
2. Enable `-development` flag for verbose logging
3. Check build report: `Build.WriteReport()` output
4. Verify Build Settings window matches expected configuration
5. Check platform-specific settings (Android signing, iOS team ID, etc.)

## Conditional Addressables (v1.2.1+)

**Schema-based only** (no prefix-based logic). Attach schemas to addressable groups in Unity Inspector.

**IncludeInBuildWithSymbolSchema**: Include group only when defines present
- Add schema to group → Configure symbols list → Choose "Require All" or "Any"
- Example: Include debug assets only when `DEBUG_UI` defined

**ExcludeInBuildWithSymbolSchema**: Exclude group when defines present
- Add schema to group → Configure symbols list → Choose "Exclude If Any" or "All"
- Example: Exclude test levels when `PRODUCTION` defined

**Testing**: Use `Foundation/Build/Test Conditional` menu items:
- Show Current Symbol Status
- Test Build And Log Groups
- Simulate PRODUCTION Build

## CI/CD Integration

### GitHub Actions Workflow
`.github/workflows/upm-publish-dispatcher.yml` - Triggers on package.json changes
- Detects version bumps in package.json
- Dispatches to The1Studio/UPMAutoPublisher for automated publishing
- Publishes to https://upm.the1studio.org/ registry

### Publishing Flow
1. Commit package.json version bump → 2. Push to master/main → 3. Dispatcher detects change → 4. Triggers UPMAutoPublisher → 5. Publishes to registry

## Build/Addressables Separation

### Two Independent Build Commands

The build system uses **two separate commands** that you combine for a full build:

**1. Player Build (BuildFromCommandLine)**
```bash
Unity -executeMethod Build.BuildFromCommandLine -platforms "android"
# Builds: Player ONLY (addressables NOT included)
```

**2. Addressables Build (BuildAddressablesOnly)**
```bash
Unity -executeMethod Build.BuildAddressablesOnly
# Builds: Addressables ONLY (no player)
```

**3. Full Build = 1 + 2**
```bash
# Run both commands for a complete build
Unity -executeMethod Build.BuildAddressablesOnly
Unity -executeMethod Build.BuildFromCommandLine -platforms "android"
```

### CI/CD Pipeline Optimization

Separate builds enable efficient CI/CD workflows:

**Scenario 1: Content-Only Update (No Code Changes)**
```bash
# Step 1: Build only addressables (fast, 1-5 minutes)
Unity -executeMethod Build.BuildAddressablesOnly \
  -remoteAddressableBuildPath "ServerData/[BuildTarget]" \
  -remoteAddressableLoadPath "https://cdn.example.com/[BuildTarget]"

# Step 2: Upload ServerData/ to CDN
aws s3 sync ServerData/ s3://my-cdn-bucket/addressables/

# No player build needed! Users get new content via remote catalog update
```

**Scenario 2: Code + Content Update (Full Release)**
```bash
# Step 1: Build addressables
Unity -executeMethod Build.BuildAddressablesOnly

# Step 2: Build player
Unity -executeMethod Build.BuildFromCommandLine -platforms "android;ios"

# Step 3: Upload addressables to CDN
# Step 4: Submit player builds to stores
```

**Scenario 3: Multi-Platform with Shared Addressables**
```bash
# Step 1: Build addressables once
Unity -executeMethod Build.BuildAddressablesOnly

# Step 2: Build all platforms in parallel (all skip addressables automatically)
Unity -executeMethod Build.BuildFromCommandLine -platforms "win64" &
Unity -executeMethod Build.BuildFromCommandLine -platforms "osx" &
Unity -executeMethod Build.BuildFromCommandLine -platforms "android" &
wait

# Addressables built once, players built in parallel!
```

### When to Use Each Command

| Command | Use When | Build Time | Use Case |
|---------|----------|------------|----------|
| **BuildAddressablesOnly** | Asset changes, content updates | Fast (1-5 min) | New levels, textures, configs |
| **BuildFromCommandLine** | Code changes, app updates | Medium (5-15 min) | Bug fixes, new features |
| **Both (Full Build)** | Initial release, major updates | Longest (6-20 min) | Version 1.0, major releases |

## Important Notes

- **Not a Unity project**: No Assets/, ProjectSettings/, Packages/ folders at root. This is a UPM package.
- **Assembly references**: BuildScript.Editor references Addressables and UniTask (optional). Check BuildScript.Editor.asmdef.
- **Platform defines**: Use BuildTools.IsDefineSet() to check scripting defines, not raw PlayerSettings access.
- **Security**: Default keystore values in Build.cs are examples only. Always override via command-line args.
- **Version**: Current version 1.2.19. Update package.json and CHANGELOG.md for releases.
- **iOS version**: Default iOS target is in BuildConstants.DEFAULT_IOS_TARGET_VERSION.
- **Android large APKs**: Use AAB format (`-buildAppBundle`) or Play Asset Delivery for apps >100MB.
