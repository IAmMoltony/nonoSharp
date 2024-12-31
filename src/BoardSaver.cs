using System;
using System.IO;

namespace NonoSharp;

public static class BoardSaver
{
    public static string GetLevelSavePath()
    {
        return Path.Combine(Settings.GetDataFolderPath(), "CustomLevels");
    }

    public static void SaveBoard(Board board, string fileName, int maxHints, bool absolutePath = false)
    {
        string? saveData = board.Serialize();
        if (saveData == null)
            throw new InvalidOperationException("Board save data is null");

        string filePath = absolutePath ? $"{fileName}.nono" : Path.Combine(GetLevelSavePath(), $"{fileName}.nono");
        string? levelDirPath = Path.GetDirectoryName(filePath);
        if (levelDirPath == null)
            throw new InvalidOperationException("Cannot determine level directory path");

        board.maxHints = maxHints;
        Directory.CreateDirectory(levelDirPath);

        using StreamWriter writer = new(filePath);
        writer.WriteLine(saveData);
    }
}
