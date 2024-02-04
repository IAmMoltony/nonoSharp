using System;
using System.IO;

namespace NonoSharp;

public class BoardLoader
{
    public static void LoadBoard(ref Tile[,] board, ref int boardSize, string fileName)
    {
        string[] boardData = File.ReadAllLines(fileName);
        boardSize = int.Parse(boardData[0]);
        board = new Tile[boardSize, boardSize];

        for (int i = 1; i < boardData.Length; i++)
        {
            string row = boardData[i];
            for (int j = 0; j < boardSize; j++)
            {
                char ch = row[j];
                if (ch == '#')
                    board[j, i - 1].State = TileState.Filled;
                else
                    board[j, i - 1].State = TileState.Empty;
            }
        }
    }
}
