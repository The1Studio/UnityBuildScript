# The One Unity Build Script

A comprehensive Unity build automation package that provides command-line and menu-based build functionality for multiple platforms.

## Features

- 🎮 **Multi-Platform Support**: Build for Windows, Mac, Android, iOS, and WebGL
- 📦 **Addressables Integration**: Full support for Unity Addressables with CDN deployment
- 🔧 **Command-Line Interface**: Automate builds in CI/CD pipelines
- 🎯 **Flexible Configuration**: Extensive build customization options
- 📱 **Android Features**: AAB generation, Play Asset Delivery, custom keystores
- 🍎 **iOS Features**: Automatic Xcode project configuration, Swift support
- 🌐 **WebGL Optimization**: Compression settings and size optimization

## Installation

### Via Unity Package Manager

1. Open Unity Package Manager (Window → Package Manager)
2. Click "+" → Add package from git URL
3. Enter: `https://github.com/The1Studio/UnityBuildScript.git#upm/v1.2.1`

### Via UPM Registry

Add to your `Packages/manifest.json`:
```json
{
  "dependencies": {
    "com.theone.foundation.buildscript": "1.2.1"
  },
  "scopedRegistries": [
    {
      "name": "The1Studio",
      "url": "https://upm.the1studio.org",
      "scopes": ["com.theone"]
    }
  ]
}
```

## Quick Start

### Command Line Build

```bash
# Basic Windows build
Unity -batchmode -quit -projectPath . -executeMethod Build.BuildFromCommandLine -platforms "win64"

# Android AAB with signing
Unity -batchmode -quit -projectPath . -executeMethod Build.BuildFromCommandLine \
  -platforms "android" \
  -buildAppBundle \
  -keyStoreFileName "path/to/keystore" \
  -keyStorePassword "password" \
  -keyStoreAliasName "alias" \
  -keyStoreAliasPassword "aliaspass"

# Multiple platforms
Unity -batchmode -quit -projectPath . -executeMethod Build.BuildFromCommandLine \
  -platforms "win64;android;ios"
```

### Unity Editor Menu

Access build options from: **Foundation → Build**

Available options:
- Platform-specific builds (Windows, Mac, Android, iOS, WebGL)
- Development/Production configurations
- Mono/IL2CPP scripting backends
- Addressables management

## Command-Line Arguments

| Argument | Description | Example |
|----------|-------------|---------|
| `-platforms` | Target platforms (semicolon-separated) | `"win64;android"` |
| `-development` | Enable development build | `-development` |
| `-outputPath` | Custom output directory | `-outputPath "builds/output"` |
| `-buildAppBundle` | Build Android AAB | `-buildAppBundle` |
| `-optimizeSize` | Enable size optimization | `-optimizeSize` |
| `-packageName` | Override package name | `-packageName "com.company.game"` |
| `-remoteAddressableBuildPath` | Addressables server path | `-remoteAddressableBuildPath "ServerData/[BuildTarget]"` |
| `-remoteAddressableLoadPath` | Addressables CDN URL | `-remoteAddressableLoadPath "https://cdn.example.com"` |

### Android-Specific Arguments

| Argument | Description |
|----------|-------------|
| `-keyStoreFileName` | Path to keystore file |
| `-keyStorePassword` | Keystore password |
| `-keyStoreAliasName` | Keystore alias |
| `-keyStoreAliasPassword` | Alias password |

### iOS-Specific Arguments

| Argument | Description |
|----------|-------------|
| `-iosTargetOSVersion` | Minimum iOS version |
| `-iosSigningTeamId` | Apple Developer Team ID |

## Platform Support

| Platform | Supported | Notes |
|----------|-----------|-------|
| Windows (32/64-bit) | ✅ | Full support |
| macOS | ✅ | Intel and Apple Silicon |
| Android | ✅ | APK/AAB, Play Asset Delivery |
| iOS | ✅ | Xcode project configuration |
| WebGL | ✅ | Compression optimization |

## Advanced Features

### Addressables Configuration

The package automatically configures Addressables for production builds:
- Remote catalog support
- CDN deployment ready
- Compression settings per platform
- Build path customization

### Conditional Addressable Groups (v1.2.1+)

**Schema-based conditional inclusion/exclusion of addressable groups.**

Starting from v1.2.1, conditional addressable groups are controlled exclusively through schemas, not by group name prefixes.

#### IncludeInBuildWithSymbolSchema
Add this schema to a group to include it only when specific scripting define symbols are set:
- Add multiple symbols to check
- Choose "Require All" (all symbols must be present) or "Any" (at least one symbol) logic
- Group automatically included/excluded based on current defines

#### ExcludeInBuildWithSymbolSchema
Add this schema to a group to exclude it when specific scripting define symbols are set:
- Add multiple symbols to check
- Choose "Exclude If Any" (exclude if any symbol present) or "All" (exclude only if all present) logic
- Useful for excluding debug/test content from production builds

#### How to Use
1. Select an Addressable Group in Unity
2. In the Inspector, click "Add Schema"
3. Choose either `IncludeInBuildWithSymbolSchema` or `ExcludeInBuildWithSymbolSchema`
4. Configure the symbols in the schema settings
5. The group will automatically be included/excluded during builds based on active symbols

#### Example Use Cases
- Include debug UI assets only when `DEBUG_UI` is defined
- Exclude test levels when `PRODUCTION` is defined
- Include platform-specific assets based on platform defines
- Conditionally include developer tools based on `DEVELOPER_BUILD`

#### Testing
Use the test menu items under **Build → Test Conditional** to verify your setup:
- **Show Current Symbol Status**: View all active symbols
- **Test Build And Log Groups**: See which groups have schemas and their include/exclude state
- **Simulate PRODUCTION Build**: Test with PRODUCTION symbol active

### Scripting Define Symbols

Use the `BuildTools.IsDefineSet()` utility to check scripting defines:

```csharp
if (BuildTools.IsDefineSet("MY_DEFINE"))
{
    // Custom build logic
}
```

### Build Callbacks

Implement custom pre/post-build logic by extending the build pipeline.

## Requirements

- Unity 2022.3 or higher
- Addressables package (1.22.2+)
- Platform-specific modules installed

## Support

- **Issues**: [GitHub Issues](https://github.com/The1Studio/UnityBuildScript/issues)
- **Documentation**: [Wiki](https://github.com/The1Studio/UnityBuildScript/wiki)
- **Registry**: https://upm.the1studio.org/

## License

MIT License - See LICENSE file for details

## Changelog

See [CHANGELOG.md](CHANGELOG.md) for version history and updates.