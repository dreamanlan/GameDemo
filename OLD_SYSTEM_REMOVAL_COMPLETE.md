# StoryScript Refactoring - Old System Removal Complete

## Summary

The old `IStoryCommand`/`IStoryFunction` system has been successfully removed from the codebase. All references have been updated or deleted.

## Changes Made

### Files Deleted
- `StoryDefine.cs` - Contains old enums `StoryCommandGroupDefine` and `StoryFunctionGroupDefine`
- `ForPPT/` directory - Contains old VisualStoryEditor that depended on old system
- `backup/` directory - Contains old backup files

### Files Modified

#### `SceneSystem.cs`
- Removed calls to `StoryCommandManager.Instance.GenCommandDocs()` and `StoryFunctionManager.Instance.GenFunctionDocs()`
- Now initializes `m_CommandDocs` and `m_FunctionDocs` as empty lists

#### `DslCalculatorApi.cs`
- Removed `USE_GM_STORY` section that contained `StoryVarExp`, `StoryFunctionExp`, `StoryCommandExp` classes
- Removed registration of `storyvar`, `storyfunction`, `storycommand` expressions
- Commented out `#define USE_GM_STORY`

#### `ClientGmStorySystem.cs`
- Removed old command registrations in `Init()` method
- Added comment explaining the old system was removed

#### `StoryConfigManager.cs`
- Already modified in previous session to skip "command" and "value" blocks

### Build Verification
- ✅ StoryScript.csproj - Builds successfully
- ✅ GameCore.csproj - Builds successfully

## Remaining Tasks (from original plan)

### Step 2: `this.name` syntax support
- Not started
- Needed for accessing StoryInstance data members in DSL

### Step 3: `@@var` cross-thread variable store
- Not started
- Needed for global variables accessible from multiple threads

These tasks are not related to removing the old system and can be implemented separately.
