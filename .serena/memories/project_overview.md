# Unity Build Script Project Overview

## Purpose
The One Unity Build Script (com.theone.foundation.buildscript) is a Unity Package Manager (UPM) package that provides automated build functionality for Unity projects across multiple platforms.

## Package Information
- **Package Name**: com.theone.foundation.buildscript
- **Version**: 1.0.10
- **Unity Version**: 2022.3+
- **Repository**: https://github.com/The1Studio/UnityBuildScript

## Tech Stack
- **Language**: C# 
- **Framework**: Unity Engine 2022.3+
- **Build System**: Unity's built-in build pipeline
- **Asset Management**: Unity Addressables
- **Dependencies**: 
  - Unity.Addressables (1.22.2)
  - Unity.ScriptableBuildPipeline
  - UniTask (optional, when available)

## Supported Platforms
- Windows (32-bit and 64-bit)
- macOS
- Android (APK and AAB)
- iOS
- WebGL

## Build Configurations
- **Scripting Backends**: Mono2x and IL2CPP
- **Build Options**: Development/Production builds
- **Compression**: LZ4/LZ4HC/LZMA support
- **Addressables**: Support for CDN-based remote content delivery