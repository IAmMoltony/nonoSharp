using System.Diagnostics;
using System.Reflection;

namespace NonoSharp;

public static class GameVersion
{
    public static string GetGameVersion()
    {
        Assembly asm = Assembly.GetExecutingAssembly();
        FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(asm.Location);
        return fvi.FileVersion ?? "0.0.0";
    }
}
