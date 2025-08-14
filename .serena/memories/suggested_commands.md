# Suggested Commands for Unity Build Script Development

## Unity Build Commands (Command Line)

### Basic Build Commands
```bash
# Build for specific platform
Unity -batchmode -quit -projectPath . -executeMethod Build.BuildFromCommandLine -platforms "win64"

# Build with custom output path
Unity -batchmode -quit -projectPath . -executeMethod Build.BuildFromCommandLine -platforms "android" -outputPath "builds/game.apk"

# Development build
Unity -batchmode -quit -projectPath . -executeMethod Build.BuildFromCommandLine -platforms "win64" -development

# Android AAB build
Unity -batchmode -quit -projectPath . -executeMethod Build.BuildFromCommandLine -platforms "android" -buildAppBundle -outputPath "builds/game.aab"
```

### Command Line Arguments
- `-platforms`: Target platform(s) separated by semicolon (win32, win64, osx, android, ios, webgl)
- `-development`: Enable development build
- `-outputPath`: Output file path
- `-buildAppBundle`: Build Android App Bundle (AAB)
- `-optimizeSize`: Enable size optimization
- `-packageName`: Override package name
- `-keyStoreFileName`: Android keystore file
- `-keyStorePassword`: Keystore password
- `-keyStoreAliasName`: Keystore alias
- `-keyStoreAliasPassword`: Alias password
- `-iosTargetOSVersion`: iOS minimum OS version
- `-iosSigningTeamId`: iOS signing team ID
- `-remoteAddressableBuildPath`: Addressable build path
- `-remoteAddressableLoadPath`: Addressable load path

## Unity Editor Menu Commands
Access via Unity Editor menu: **Foundation/Build/**

- Build Win32/Win64 (Mono/IL2CPP)
- Build Mac
- Build Android (Mono/IL2CPP/AAB)
- Build WebGL (Mono/IL2CPP)
- Build All Platforms
- Setup Android Keystore
- Build Addressables (Fresh)
- Set Scripting Define Symbols
- Open Build Log

## Development Commands

### Git Operations
```bash
# Clone the repository
git clone git@github.com:The1Studio/UnityBuildScript.git

# Check status
git status

# View recent commits
git log --oneline -10
```

### Unity Package Management
```bash
# Open Unity with this project
Unity -projectPath .

# Open Unity in batch mode for testing
Unity -batchmode -projectPath . -logFile -
```

## Testing & Validation
- Open Unity Editor and check Console for compilation errors
- Use Unity MCP tools: `mcp__UnityMCP__read_console`
- Verify build menu items appear under Foundation/Build/
- Test build for each platform through Editor menu

## File System Navigation
```bash
# List project structure
ls -la Assets/BuildScripts/Editor/

# Find build-related files
find . -name "*.cs" -path "*/BuildScripts/*"

# Search for specific build configurations
grep -r "BuildOptions" Assets/BuildScripts/
```