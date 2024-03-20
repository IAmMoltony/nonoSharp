
# nonoSharp

nonoSharp is a Nonogram game written in C#.

[![CodeFactor](https://www.codefactor.io/repository/github/iammoltony/nonosharp/badge)](https://www.codefactor.io/repository/github/iammoltony/nonosharp)

## Dependencies

The only dependency that has to be installed is the .NET SDK. All libraries are installed during the build phase.

## Building and Running

<!--
This is for later if we do actually get shaders

### Non-Windows Platforms

If you use Linux or macOS on your machine, follow the following steps to install DirectX shader compiler.

1. Install Winetricks:
  - Ubuntu: `sudo apt install -y winetricks`
  - Arch: `sudo pacman -S --needed winetricks`
  - Fedora: `sudo dnf install winetricks` (I believe)
  - macOS Homebrew: `brew install winetricks`
1. Run Winetricks (`winetricks` command)
1. In Winetricks select "Create new wineprefix", select architecture to **64**, pick whatever name and click OK.
  - Please don't include spaces in the prefix name. It *might* work with spaces but I don't recommend it.
  - If you choose architecture as 32, it will not work.
  - If this gives the following error: `warning: wine cmd.exe /c echo '%AppData%' returned empty string, error message ""`, try launching it like this: `WINEARCH=win64 WINEPREFIX=<Prefix directory, usually ~/.local/share/wineprefixes/prefix-name-here> winetricks`
1. Install Direct3D Compiler: Install a Windows DLL or component -> Select `d3dcompiler_47` -> Click OK and wait until it installs
  - If download fails with `aria2c` saying that the cause is "Permission denied", then you can try setting `WINETRICKS_DOWNLOADER` to `wget` or another downloader program. This usually happens when you used `snap` to install `aria2c`.
    - So now launching Winetricks should be done like this: `WINETRICKS_DOWNLOADER=wget winetricks`
    - If you used the previous workaround, don't forget to add all the other variables.
1. .NET 6 installation time: [go here](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) and download the installer for *Windows x64*
1. Select "Run command line shell (for debugging)" and press OK. This will take you to a terminal.
1. Navigate to wherever you downloaded the installer (usually `~/Downloads`)
1. `wine (installer name, usually dotnet-sdk-version-win-x64.exe)`
1. Install .NET (it might take quite a bit of time so wait patiently)
1. Close the terminal where you started the installer
1. Close Winetricks

Your Wine environment is now ready. Now it's just a matter of telling MonoGame where that environment is.

1. Find out what shell you're using: `which $SHELL`
1. With that in mind, open your shell's configuration file in a text editor:
  - `bash`: Open `~/.bashrc`
  - `csh`: Open `~/.cshrc`
  - `fish`: Open `~/.fishrc`
  - `ash`: Open `~/.ashrc`
  - `zsh`: Open `~/.zshrc`
  - Other shells: Usually the config is called `.(Shell name)rc` and is located in the home directory.
1. At the end of the file append this: `export MGFXC_WINE_PATH=$HOME/.local/share/wineprefixes/prefix-name-here`, don't forget to replace the prefix name with the actual prefix name.
1. Reload your shell by closing the terminal (or tab) and opening it again
1. That's it! You're done.
-->

### Build nonoSharp

```bash
git clone https://github.com/IAmMoltony/nonoSharp # Clone the repository
cd nonoSharp # Go to the repository folder (or directory, whatever you want to call it)
dotnet build # Build the project
```

### Run nonoSharp

```bash
dotnet run
```

