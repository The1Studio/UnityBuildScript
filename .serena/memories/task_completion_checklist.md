# Task Completion Checklist

When completing any development task in this Unity Build Script project, ensure:

## Code Quality Checks
1. **Compilation**: Open Unity Editor and verify no compilation errors in Console
2. **Unity MCP Check**: Use `mcp__UnityMCP__read_console` to verify no errors
3. **Menu Items**: Verify new menu items appear correctly in Foundation/Build/
4. **Assembly References**: Ensure correct assembly references in .asmdef files

## Testing Requirements
1. **Editor Testing**: Test build menu items work in Unity Editor
2. **Command Line Testing**: Test command-line builds with sample parameters
3. **Platform Testing**: Verify builds succeed for target platforms
4. **Addressables**: Test addressable builds if modified

## Before Committing
1. **No Compilation Errors**: Unity project must compile without errors
2. **Version Update**: Update version in package.json if adding features
3. **No Sensitive Data**: Never commit API keys, passwords, or keystores
4. **Clean Working Directory**: Remove temporary build files

## Documentation Updates
1. Update package.json version and description if needed
2. Document new command-line arguments if added
3. Update menu structure if modified

## Validation Commands
```bash
# Check Unity compilation (use Unity MCP)
mcp__UnityMCP__read_console

# Verify git status
git status

# Check for large files that shouldn't be committed
find . -size +10M -not -path "./.git/*"
```

## Platform-Specific Validation
- **Android**: Verify keystore configuration works
- **iOS**: Check Xcode project generates correctly
- **WebGL**: Verify compression settings applied
- **Addressables**: Confirm CDN profile creates/updates

## Final Steps
1. Test at least one full build cycle
2. Verify build output in designated folder
3. Check build report for errors/warnings
4. Ensure all modified files are intentional changes