# nonoInstaller: The nonoSharp Windows installer

Name "nonoSharp"
Caption "nonoInstaller"
Icon "Icon.ico"
OutFile "nonoInstaller.exe"

InstallDir "$PROGRAMFILES\nonoSharp"

RequestExecutionLevel admin

Page license
Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

ShowInstDetails show

Section ""
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\nonoSharp" "DisplayName" "nonoSharp"
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\nonoSharp" "UninstallString" "$INSTDIR\nonoUninstall.exe"

    SetOutPath "$INSTDIR"
    WriteUninstaller "$INSTDIR\nonoUninstall.exe"
SectionEnd

Section "CopyFiles"
    File /r "bin\Release\net8.0\win-x64\publish"
SectionEnd

UninstallText "Click 'Next' to uninstall nonoSharp."
UninstallIcon "Icon.ico"

Section "Uninstall"
    DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\nonoSharp"
    RMDir "$INSTDIR"
SectionEnd
