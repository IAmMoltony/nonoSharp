#!/usr/bin/env bash

dotnet publish --configuration Release --runtime linux-x64 --self-contained
dotnet publish --configuration Release --runtime win7-x64 --self-contained
