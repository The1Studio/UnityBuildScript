# Code Architecture

## Main Components

### Core Build System (Assets/BuildScripts/Editor/)
1. **Build.cs**: Main build orchestrator
   - `BuildFromCommandLine()`: Entry point for CI/CD builds
   - `BuildInternal()`: Core build logic for all platforms
   - Platform-specific configurations via `Targets` dictionary
   - Handles command-line arguments parsing

2. **BuildMenu.cs**: Unity Editor integration
   - Menu items under "Foundation/Build/"
   - Platform-specific build methods (Build_Android_*, Build_Win64, etc.)
   - Quick access to common build configurations

3. **BuildTools.cs**: Utility functions
   - Assembly management
   - Build settings reset
   - Editor user build settings management

4. **AddressableBuildTool.cs**: Addressable asset system
   - Build addressables with platform optimization
   - CDN profile management (TheOneCDN)
   - Compression mode configuration (LZ4/LZMA)

5. **IOSPostProcessingBuildTool.cs**: iOS-specific post-processing
   - Xcode project configuration
   - Swift support settings
   - Entitlements and capabilities management

### Runtime Components (Assets/GameVersion/)
- **GameVersion.cs**: Version management at runtime
- Assembly: BuildScript.Runtime

## Build Flow
1. Parse command-line arguments or menu selection
2. Configure platform-specific settings
3. Set scripting define symbols
4. Configure Android keystore (if Android)
5. Configure iOS signing (if iOS)  
6. Build addressables (if enabled)
7. Execute Unity build pipeline
8. Post-process build output
9. Generate build report

## Assembly Structure
- **BuildScript.Editor**: Editor-only build tools
- **BuildScript.Runtime**: Runtime version info
- Conditional compilation: PAD define for Play Asset Delivery support