using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace NonoSharp;

public class LevelList : IEnumerable<LevelMetadata>
{
    public List<LevelMetadata> Levels { get; private set; }

    public LevelList()
    {
        Levels = new();
    }

    public void FindLevels()
    {
        string levelsDir = AppDomain.CurrentDomain.BaseDirectory + "/Content/Levels";
        DirectoryInfo dirInfo = new(levelsDir);
        FileInfo[] files = dirInfo.GetFiles("*.nono");
        for (int i = 0; i < files.Length; i++)
        {
            string sizeStr = File.ReadAllLines($"{levelsDir}/{files[i].Name}")[0];
            if (!int.TryParse(sizeStr, out int size))
            {
                Log.Logger.Warning($"Level {files[i].Name} does not have a valid board size, skip");
                continue;
            }
            Levels.Add(new(Path.GetFileNameWithoutExtension(files[i].Name), size));
            Log.Logger.Information($"Found level: {Levels[^1]}");
        }
    }

    public IEnumerator<LevelMetadata> GetEnumerator()
    {
        return Levels.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public LevelMetadata this[int index]
    {
        get { return Levels[index]; }
        set { Levels[index] = value; }
    }
}
