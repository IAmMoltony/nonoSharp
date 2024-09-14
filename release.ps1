#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Script for packaging nonoSharp.
.DESCRIPTION
    This is a script for packaging nonoSharp for Linux and Windows.
.PARAMETER GameVersion
    Specify the version of the game. This only affects the file name of the zip files generated.
.PARAMETER NoBuild
    Don't build the game solution.
.PARAMETER NoColor
    Disable colored output.
.PARAMETER Verbosity
    The verbosity of .NET commands. Allowed values: q, m, n, d, diag. Default is 'm'.
.EXAMPLE
    PS /home/moltony/dev/nonoSharp> ./release.ps1 0.11.1
#>

param (
    [Parameter(Mandatory=$true, ParameterSetName="GameVersion", Position=0)][string]$GameVersion,
    [switch]$NoBuild,
    [switch]$NoColor,
    [Parameter(Mandatory=$false)][ValidateSet("q", "m", "n", "d", "diag")][string]$Verbosity = "m"
)

# stop on error
$ErrorActionPreference = "Stop"

# check pwsh version
if ($PSVersionTable.PSVersion.Major -lt 5 -and $PSVersionTable.PSVersion.Minor -lt 1) {
    Write-Host "Warning: you are using a version of PowerShell older than 5.1. This script is currently designed to run on PowerShell 7 and above, expect problems!"
}

if (!$NoBuild) {
    if ($NoColor) {
        Write-Host "  *** Cleaning up ***"
    } else {
        Write-Host "  *** `e[0;32mCleaning up`e[0m ***"
    }

    # remove all these folders (if they exist)
    $PathsToRemove = "./linux-build", "./windows-build", "./Content/bin", "./Content/obj"
    foreach ($Path in $PathsToRemove) {
        if (Test-Path $Path) {
            Remove-Item $Path -Recurse -Force
        }
    }

    # clean project
    dotnet clean -v $Verbosity

    # build for linux
    if ($NoColor) {
        Write-Host "  *** Building for Linux x64 ***"
    } else {
        Write-Host "  *** `e[0;32mBuilding for `e[0;33mLinux x64`e[0m ***"
    }
    dotnet publish -v $Verbosity --configuration Release --runtime linux-x64 --self-contained
    Copy-Item ./bin/Release/net8.0/linux-x64/publish ./linux-build

    # build for windows
    if ($NoColor) {
        Write-Host "  *** Building for Windows x64 ***"
    } else {
        Write-Host "  *** `e[0;32mBuilding for `e[0;33mWindows x64`e[0m ***"
    }
    dotnet publish -v $Verbosity --configuration Release --runtime win-x64 --self-contained
    Copy-Item ./bin/Release/net8.0/win-x64/publish ./windows-build
}

# zip linux build
if ($NoColor) {
    Write-Host "  *** Zipping Linux build ***"
} else {
    Write-Host "  *** `e[0;32mZipping `e[0;33mLinux`e[0;32m build`e[0m ***"
}
Compress-Archive -Path ./linux-build -DestinationPath ./nonoSharpLinux$GameVersion.zip -Force

# zip windows build
if ($NoColor) {
    Write-Host "  *** Zipping Windows build ***"
} else {
    Write-Host "  *** `e[0;32mZipping `e[0;33mWindows`e[0;32m build`e[0m ***"
}
Compress-Archive -Path ./windows-build -DestinationPath ./nonoSharpWindows$GameVersion.zip -Force

# move builds to ReleaseBuilds/
New-Item -Path ./ReleaseBuilds -ItemType Directory -Force
Move-Item nonoSharpLinux$GameVersion.zip ./ReleaseBuilds -Force
Move-Item nonoSharpWindows$GameVersion.zip ./ReleaseBuilds -Force

if ($NoColor) {
    Write-Host "  *** Done! ***"
} else {
    Write-Host "  *** `e[0;32mDone!`e[0m ***"
}
