using System;
using System.IO;

namespace NonoSharp;

public struct LevelMetadata
{
    public string name;
    public int size;
    public bool isCustomLevel;

    public LevelMetadata(string name, int size, bool isCustomLevel)
    {
        this.name = name;
        this.size = size;
        this.isCustomLevel = isCustomLevel;
    }

    public LevelMetadata()
    {
        name = "";
        size = 0;
        isCustomLevel = false;
    }

    public override readonly string ToString() => $"{name} ({size}x{size})";

    public readonly string GetPath() => Path.Combine(isCustomLevel ? BoardSaver.GetLevelSavePath() : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "Levels"), $"{name}.nono");
}
