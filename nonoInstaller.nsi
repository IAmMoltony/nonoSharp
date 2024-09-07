# nonoInstaller: The nonoSharp Windows installer

!include "x64.nsh"
!include "MUI2.nsh"

Name "nonoSharp"
Caption "nonoInstaller"
Icon "InstallerIcon.ico"
OutFile "nonoInstaller.exe"

VIProductVersion "0.13.0.0"
VIFileVersion "0.13.0.0"
VIAddVersionKey "FileVersion" "0.13.0.0"
VIAddVersionKey "LegalCopyright" "(c) moltony"
VIAddVersionKey "FileDescription" "nonoSharp"

InstallDir "$PROGRAMFILES64\nonoSharp"
RequestExecutionLevel admin

!define MUI_ABORTWARNING
!define MUI_FINISHPAGE_NOAUTOCLOSE
!define MUI_FINISHPAGE_RUN "$INSTDIR\nonoSharp.exe"
!define MUI_FINISHPAGE_RUN_NOTCHECKED
!define MUI_FINISHPAGE_NOREBOOTSUPPORT
!define MUI_ICON "InstallerIcon.ico"

!define MUI_WELCOMEPAGE_TITLE "Welcome to nonoInstaller!"
!define MUI_WELCOMEPAGE_TEXT "This program will now install nonoSharp on your computer. Click the 'Next' button to continue."
!define MUI_LICENSEPAGE_TEXT_TOP "Please review the license agreement."
!define MUI_LICENSEPAGE_TEXT_BOTTOM "If you accept these terms, click the 'I Agree' button to continue. You have to accept the agreement to install nonoSharp."
!define MUI_DIRECTORYPAGE_TEXT_TOP "nonoInstaller will install nonoSharp in this folder. If you want to install in a different folder, click the 'Browse' button and select a folder. When you're ready, click 'Install'."
!define MUI_FINISHPAGE_TITLE "nonoSharp is now installed!"
!define MUI_FINISHPAGE_TEXT "nonoSharp has been installed on your computer. Click on the 'Finish' button to exit nonoInstaller."

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

Section "Copy Game Files"
    File /r "bin\Release\net8.0\win-x64\publish"
SectionEnd

Section "Add Desktop Shortcut"
    CreateShortCut "$DESKTOP\nonoSharp.lnk" "$INSTDIR\nonoSharp.exe" "" "$INSTDIR/Icon.ico"
SectionEnd

UninstallText "Click 'Next' to uninstall nonoSharp."
UninstallIcon "UninstallerIcon.ico"

Section "Uninstall"
    DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\nonoSharp"
    RMDir "$INSTDIR"
SectionEnd

!insertmacro MUI_LANGUAGE "English"
