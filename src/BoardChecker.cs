namespace NonoSharp;

using System;
using System.Collections.Generic;
using System.Linq;

public static class BoardChecker
{
    public static bool CheckBoard(Tile[,] tiles, Clues clues)
    {
        int rows = tiles.GetLength(0);
        int cols = tiles.GetLength(1);

        Tile[,] boardCopy = flipBoard((Tile[,])tiles.Clone());

        bool changed;
        do
        {
            changed = false;

            // per row
            for (int r = 0; r < rows; r++)
            {
                bool rowChanged = applyLogicalStep(boardCopy, clues.RowClues[r], r, true);
                changed |= rowChanged;
            }

            // per column
            for (int c = 0; c < cols; c++)
            {
                bool colChanged = applyLogicalStep(boardCopy, clues.ColumnClues[c], c, false);
                changed |= colChanged;
            }
        } while (changed);

        bool isValid = isBoardSolved(boardCopy, clues);
        Console.WriteLine($"Validation result: {isValid}");
        return isValid;
    }

    private static bool applyLogicalStep(Tile[,] tiles, List<int> clues, int index, bool isRow)
    {
        int length = isRow ? tiles.GetLength(1) : tiles.GetLength(0);
        int totalFilled = clues.Sum() + clues.Count - 1;
        bool changed = false;

        Console.WriteLine($"Processing {(isRow ? "row" : "column")} {index} with clues {string.Join(",", clues)}");

        // if sum of clues + spaces equals length, fill guaranteed tiles
        if (totalFilled == length)
        {
            Console.WriteLine($"{(isRow ? "Row" : "Column")} {index} has guaranteed fills.");
            int pos = 0;
            foreach (int clue in clues)
            {
                for (int i = 0; i < clue; i++)
                {
                    if (isRow && tiles[index, pos].state == TileState.Empty)
                    {
                        tiles[index, pos].state = TileState.Filled;
                        changed = true;
                    }
                    else if (!isRow && tiles[pos, index].state == TileState.Empty)
                    {
                        tiles[pos, index].state = TileState.Filled;
                        changed = true;
                    }
                    pos++;
                }
                if (pos < length)
                {
                    tiles[index, pos].state = TileState.Cross; // 1000% empty space
                    pos++;
                }
            }
        }
        return changed;
    }

    private static bool isBoardSolved(Tile[,] tiles, Clues clues)
    {
        for (int r = 0; r < tiles.GetLength(0); r++)
        {
            if (!isRowValid(tiles, clues.RowClues[r], r))
            {
                return false; // its invalid
            }
        }
        for (int c = 0; c < tiles.GetLength(1); c++)
        {
            if (!isColumnValid(tiles, clues.ColumnClues[c], c))
            {
                return false; // its invalid
            }
        }
        return true;
    }

    private static bool isRowValid(Tile[,] tiles, List<int> clues, int row)
    {
        return checkLineValidity(tiles, clues, row, true);
    }

    private static bool isColumnValid(Tile[,] tiles, List<int> clues, int col)
    {
        return checkLineValidity(tiles, clues, col, false);
    }

    private static bool checkLineValidity(Tile[,] tiles, List<int> clues, int index, bool isRow)
    {
        List<int> foundGroups = new();
        int count = 0;
        int length = isRow ? tiles.GetLength(1) : tiles.GetLength(0);

        for (int i = 0; i < length; i++)
        {
            TileState state = isRow ? tiles[index, i].state : tiles[i, index].state;
            if (state == TileState.Filled) count++;
            else if (count > 0)
            {
                foundGroups.Add(count);
                count = 0;
            }
        }
        if (count > 0) foundGroups.Add(count);

        if (clues.Count == 1 && clues[0] == 0)
        {
            bool isCompletelyEmpty = foundGroups.Count == 0;
            // expected empty line
            return isCompletelyEmpty;
        }
        return foundGroups.SequenceEqual(clues);
    }

    private static Tile[,] flipBoard(Tile[,] board)
    {
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);
        Tile[,] flipped = new Tile[cols, rows];

        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                flipped[c, r] = board[r, c];

        return flipped;
    }
}

