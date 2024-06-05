using System;
using System.IO;

namespace NonoSharp;

public static class BoardSaver
{
    public static string GetLevelSavePath()
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "nonoSharp", "CustomLevels");
    }

    public static void SaveBoard(Board board, string fileName, int maxHints)
    {
        board.maxHints = maxHints;
        string saveData = board.Serialize();
        string filePath = Path.Combine(GetLevelSavePath(), $"{fileName}.nono");
        using StreamWriter writer = new(filePath);
        writer.WriteLine(saveData);
    }
}
