#!/usr/bin/env bash

set -e

if [ "$1" == "" ]; then
    echo -e "\033[0;36mUsage\033[0m: $0 \033[0;33m<game version>\033[0m\ne.g.: $0 \033[0;33m0.8.0\033[0m"
	exit 1
fi

echo -e "  *** \033[0;32mCleaning up\033[0m ***"
rm -rf linux-build windows-build Content/bin Content/obj
dotnet clean -v m

echo -e "  *** \033[0;32mBuilding for \033[0;33mLinux x64\033[0m ***"
dotnet publish -v m --configuration Release --runtime linux-x64 --self-contained
rm bin/Release/net6.0/linux-x64/publish -r
mv bin/Release/net6.0/linux-x64 linux-build

echo -e "  *** \033[0;32mBuilding for \033[0;33mWindows 7\033[0m x64 ***"
dotnet publish -v m --configuration Release --runtime win7-x64 --self-contained
rm bin/Release/net6.0/win7-x64/publish -r
mv bin/Release/net6.0/win7-x64 windows-build

echo -e "  *** \033[0;32mZipping Linux build\033[0m ***"
cd linux-build
zip -r ../nonoSharpLinux"$1".zip -xi ./*

echo -e "  *** \033[0;32mZipping Windows build\033[0m ***"
cd ../windows-build
zip -r ../nonoSharpWindows"$1".zip -xi ./*

mkdir -p ReleaseBuilds
mv nonoSharpLinux"$1".zip ReleaseBuilds/
mv nonoSharpWindows"$1".zip ReleaseBuilds/
