#!/usr/bin/env bash

set -e

echo " ** Cleaning up **"
rm -rf linux-build windows-build
rm -rf Content/bin Content/obj
dotnet clean -v m

echo " ** Building for Linux x64 **"
dotnet publish -v m --configuration Release --runtime linux-x64 --self-contained
rm bin/Release/net6.0/linux-x64/publish -r
mv bin/Release/net6.0/linux-x64 linux-build

echo " ** Building for Windows 7 x64 **"
dotnet publish -v m --configuration Release --runtime win7-x64 --self-contained
rm bin/Release/net6.0/win7-x64/publish -r
mv bin/Release/net6.0/win7-x64 windows-build

echo " ** Zipping Linux build **"
cd linux-build
zip -r ../nonoSharpLinux$1.zip -xi ./*

echo " ** Zipping Windows build **"
cd ../windows-build
zip -r ../nonoSharpWindows$1.zip -xi ./*
