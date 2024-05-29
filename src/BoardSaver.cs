using System;
using System.IO;

namespace NonoSharp;

public static class BoardSaver
{
    public static void SaveBoard(Board board, string fileName, int maxHints)
    {
        board.maxHints = maxHints;
        string saveData = board.Serialize();
        string filePath = $"{AppDomain.CurrentDomain.BaseDirectory}/Content/Levels/{fileName}.nono";
        using StreamWriter writer = new(filePath);
        writer.WriteLine(saveData);
    }
}
