@echo off
if NOT "%1" EQU "" (
  set is_pause=%1
) else (
  set is_pause=True
)

del /f/q StoryDsl.txt story.json dsl.json keywords.xml ids.xml dsl.tmLanguage uew.txt dsl.uew

DocGenerator.exe

rem xcopy StoryDsl.txt ..\Release\ /y/d
xcopy dsl.json ..\vscode\dsl\snippets\ /y/d
xcopy dsl.tmLanguage ..\vscode\dsl\syntaxes\ /y/d
xcopy dsl.uew ..\ultraedit\ /y/d

xcopy StoryDsl.txt ..\..\..\..\Document\ /y/d
xcopy ..\vscode %USERPROFILE%\.vscode\extensions\ /s/y/d
xcopy dsl.uew %APPDATA%\IDMComp\UltraEdit\wordfiles\ /y/d

if %is_pause% EQU True (
  @pause
  @exit /b %ec%
)
