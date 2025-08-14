# Code Style and Conventions

## C# Coding Standards

### Naming Conventions
- **Classes**: PascalCase (e.g., `BuildMenu`, `AddressableBuildTool`)
- **Methods**: PascalCase (e.g., `BuildFromCommandLine`, `SetupIos`)
- **Private fields**: camelCase (e.g., `keyStorePassword`, `iosSigningTeamId`)
- **Constants**: PascalCase with descriptive prefix (e.g., `PlatformAndroid`, `PlatformWin64`)
- **Menu paths**: Use forward slashes (e.g., `"Foundation/Build/Win64"`)

### Code Organization
- Place editor-only code in Editor folders
- Use assembly definitions (.asmdef) to manage dependencies
- Separate runtime and editor assemblies
- Group related functionality in static classes

### Unity-Specific Patterns
- Use `[MenuItem]` attributes for editor menu integration
- Priority values for menu ordering (e.g., `[MenuItem("path", priority = 100)]`)
- Conditional compilation with platform defines (#if UNITY_ANDROID, #if PRODUCTION)
- BuildOptions flags for build configuration

### Error Handling
- Use Unity's Debug.Log/LogError for logging
- Validate command-line arguments before use
- Check for null/empty strings in configuration
- Report build failures with detailed messages

### Platform-Specific Code
- Use preprocessor directives for platform-specific code
- Centralize platform configuration in dictionaries/maps
- Separate post-processing logic by platform
- Use Unity's built-in platform detection

### Documentation
- Minimal inline comments (code should be self-documenting)
- Method names should clearly indicate purpose
- Use descriptive variable names
- Group related methods together

### Build Configuration
- Store build settings in static fields for command-line access
- Use BuildTargetInfo structs for platform metadata
- Implement platform-specific actions in separate methods
- Generate build reports for debugging