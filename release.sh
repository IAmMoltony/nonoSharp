#!/usr/bin/env bash

set -e

rm -rf linux-build windows-build
dotnet clean

echo "* Building for Linux x64 *"
dotnet publish -v m --configuration Release --runtime linux-x64 --self-contained
rm bin/Release/net6.0/linux-x64/publish -r
mv bin/Release/net6.0/linux-x64 linux-build

echo "* Building for Windows 7 x64 *"
dotnet publish -v m --configuration Release --runtime win7-x64 --self-contained
rm bin/Release/net6.0/win7-x64/publish -r
mv bin/Release/net6.0/win7-x64 windows-build

echo "* Zipping Linux build *"
zip -r nonoSharpLinux.zip -xi linux-build/*

echo "* Zipping Windows build *"
zip -r nonoSharpWindows.zip -xi windows-build/*
