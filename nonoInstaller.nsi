# nonoInstaller: The nonoSharp Windows installer

!include "MUI2.nsh"

Name "nonoSharp"
Caption "nonoInstaller"
Icon "Icon.ico"
OutFile "nonoInstaller.exe"

InstallDir "$PROGRAMFILES\nonoSharp"

RequestExecutionLevel admin

!define MUI_ABORTWARNING
!define MUI_FINISHPAGE_RUN "$INSTDIR\nonoSharp.exe"

!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_LICENSE "LICENSE"
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_UNPAGE_FINISH

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

!insertmacro MUI_LANGUAGE "English"
