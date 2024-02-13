using System.Reflection;
using System.Diagnostics;

namespace NonoSharp;

public class GameVersion
{
    private GameVersion()
    {
    }

    public static string GetGameVersion()
    {
        Assembly asm = Assembly.GetExecutingAssembly();
        FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(asm.Location);
        return fvi.FileVersion;
    }
}
