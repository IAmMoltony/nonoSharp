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
        findLevelsInDir(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "Levels"), false);
        findLevelsInDir(BoardSaver.GetLevelSavePath(), true);
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

    private void findLevelsInDir(string levelsDir, bool isCustomLevelDir)
    {
        Log.Logger.Information($"Searching for levels in folder {levelsDir} (custom: {isCustomLevelDir})");
        DirectoryInfo dirInfo = new(levelsDir);
        FileInfo[] files = new FileInfo[1];
        try
        {
            files = dirInfo.GetFiles("*.nono");
        }
        catch (DirectoryNotFoundException)
        {
            Log.Logger.Warning($"Directory {levelsDir} not found, skipping");
            return;
        }
        for (int i = 0; i < files.Length; i++)
        {
            string sizeStr = File.ReadAllLines(Path.Combine(levelsDir, files[i].Name))[0];
            if (!int.TryParse(sizeStr, out int size))
            {
                Log.Logger.Warning($"Level {files[i].Name} does not have a valid board size, skip");
                continue;
            }
            Levels.Add(new(Path.GetFileNameWithoutExtension(files[i].Name), size, isCustomLevelDir));
            Log.Logger.Information($"Found level: {Levels[^1]}");
        }
    }
}
