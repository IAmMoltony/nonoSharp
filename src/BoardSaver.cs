using System;
using System.IO;

namespace NonoSharp;

public static class BoardSaver
{
    public static string GetLevelSavePath()
    {
        // TODO add a function to get %localappdata%/nonoSharp
        // this is because right now in auto save we save it in <level save path>/..
        // which assumes that the data folder is one directory up the custom levels folder
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "nonoSharp", "CustomLevels");
    }

    public static void SaveBoard(Board board, string fileName, int maxHints)
    {
        string? saveData = board.Serialize();
        if (saveData == null)
            throw new InvalidOperationException("Board save data is null");

        string filePath = Path.Combine(GetLevelSavePath(), $"{fileName}.nono");
        string? levelDirPath = Path.GetDirectoryName(filePath);
        if (levelDirPath == null)
            throw new InvalidOperationException("Cannot determine level directory path");

        board.maxHints = maxHints;
        Directory.CreateDirectory(levelDirPath);

        using StreamWriter writer = new(filePath);
        writer.WriteLine(saveData);
    }
}
