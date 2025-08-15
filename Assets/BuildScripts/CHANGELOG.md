# Changelog

All notable changes to The One Unity Build Script package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.2.2] - 2025-01-15

### Added
- Support for `FORCE_STRIP_CODE_HIGH` scripting define symbol to force high managed stripping level in non-production builds
- This allows aggressive code stripping for size optimization testing without full production settings

### Changed
- Managed stripping level logic now checks for both `PRODUCTION` and `FORCE_STRIP_CODE_HIGH` symbols
- Engine code stripping is enabled when either symbol is present

## [1.2.1] - 2025-01-14

### Changed
- **BREAKING**: Refactored conditional addressables from prefix-based to pure schema-based approach
- Removed all hardcoded prefix logic for Debug/Creative/Editor groups
- Groups now control their own inclusion exclusively via attached schemas
- Simplified `ApplyConditionalBuildRules()` to only use schema-based rules
- Removed `ToggleGroupsByNamePrefix()` method entirely

### Added
- Comprehensive test menu items for schema-based conditional builds
- Test scenarios for PRODUCTION builds, Development builds, and symbol combinations
- Group logging functionality to visualize schema attachments and states

### Removed
- Removed prefix-based conditional logic (Debug/Creative/Editor prefix checks)
- Removed menu items for toggling specific group prefixes
- Removed `conditionalAddressables` command-line flag

### Migration Guide
- Groups are no longer controlled by their name prefix
- To control a group conditionally, attach either `IncludeInBuildWithSymbolSchema` or `ExcludeInBuildWithSymbolSchema`
- Configure the symbols in the schema inspector to control when the group is included/excluded

## [1.2.0] - 2025-01-14

### Added
- New `IncludeInBuildWithSymbolSchema` for conditional addressable group inclusion based on scripting define symbols
- New `ExcludeInBuildWithSymbolSchema` for conditional addressable group exclusion based on scripting define symbols
- Automatic schema-based conditional build rules in AddressableBuildTool
- Support for complex conditional logic with multiple symbols (require all/any)

### Changed
- Enhanced AddressableBuildTool to automatically apply schema-based conditional rules during build

## [1.1.1] - 2025-01-14

### Added
- README.md with comprehensive package documentation
- Installation instructions for both Git and UPM registry methods
- Detailed command-line usage examples
- Platform support matrix

## [1.1.0] - 2025-01-14

### Added
- New `IsDefineSet` utility methods in BuildTools for checking scripting define symbols
- Support for checking defines in specific build target groups
- Debug namespace import in BuildMenu for better logging

### Changed
- Reorganized Addressable asset groups configuration
- Updated Addressable asset group schemas with new naming conventions (creative, debug, eDitor, test)
- Improved Play Asset Delivery configuration for Android builds
- Updated project dependencies and Unity version settings

### Removed
- Removed Default Local Group addressable configuration and related schemas
- Cleaned up commented-out iOS build menu options in BuildMenu.cs

## [1.0.10] - Previous Release

### Added
- Enable deep profiling support in build script (conditional)

## [1.0.9] - Previous Release

### Changed
- Updated dependencies
- Updated Android target SDK configuration

## [1.0.8] - Previous Release

### Changed
- Updated WebGL build settings

## [1.0.7] - Previous Release

### Changed
- Updated build script for WebGL

## [1.0.6] - Previous Release

### Removed
- Removed LZMA compression handling from build script

## [1.0.5] - Previous Release

### Changed
- Updated build script to exclude PR_CHECK from production builds

## [1.0.4] - Previous Release

### Changed
- Refactored build script to streamline scripting backend handling
- Improved build options configuration

## [1.0.3] - Previous Release

### Added
- Enhanced addressable build logging

## [1.0.2] - Previous Release

### Changed
- Updated managed stripping levels for production and development builds

## Previous Versions

- Various improvements to build performance
- WebGL bug fixes
- Android build settings for Unity 6000.0 compatibility
- Compression settings adjustments for WebGL in Addressable builds
- APK splitting disabled for Unity 6000.0 or newer