namespace NonoSharp;

using System.Collections.Generic;
using System.Linq;

public static class BoardChecker
{
    public static bool CheckBoard(Tile[,] tiles, Clues clues)
    {
        int rows = tiles.GetLength(0);
        int cols = tiles.GetLength(1);

        Tile[,] tilesClone = (Tile[,])tiles.Clone();

        bool changed;
        do
        {
            changed = false;

            // per row
            for (int r = 0; r < rows; r++)
            {
                changed |= ApplyLogicalStep(tilesClone, clues.RowClues[r], r, true);
            }

            // per column
            for (int c = 0; c < cols; c++)
            {
                changed |= ApplyLogicalStep(tilesClone, clues.ColumnClues[c], c, false);
            }

        } while (changed);

        return IsBoardSolved(tilesClone, clues);
    }

    private static bool ApplyLogicalStep(Tile[,] tiles, List<int> clues, int index, bool isRow)
    {
        int length = isRow ? tiles.GetLength(1) : tiles.GetLength(0);
        int totalFilled = clues.Sum() + clues.Count - 1;
        bool changed = false;

        // if clues + spaces == length then it's guaranteed. fill em
        if (totalFilled == length)
        {
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
                    if (isRow) tiles[index, pos].state = TileState.Cross;
                    else tiles[pos, index].state = TileState.Cross;
                    pos++;
                }
            }
        }

        // if we can only fit the tile in 1 location then do it
        for (int i = 0; i < length; i++)
        {
            if (isRow && tiles[index, i].state == TileState.Empty)
            {
                if (CanOnlyBeFilled(tiles, clues, index, i, true))
                {
                    tiles[index, i].state = TileState.Filled;
                    changed = true;
                }
            }
            else if (!isRow && tiles[i, index].state == TileState.Empty)
            {
                if (CanOnlyBeFilled(tiles, clues, index, i, false))
                {
                    tiles[i, index].state = TileState.Filled;
                    changed = true;
                }
            }
        }

        return changed;
    }

    private static bool CanOnlyBeFilled(Tile[,] tiles, List<int> clues, int index, int pos, bool isRow)
    {
        int length = isRow ? tiles.GetLength(1) : tiles.GetLength(0);
        int totalFilled = clues.Sum();
        int filledCount = 0;

        for (int i = 0; i < length; i++)
        {
            TileState state = isRow ? tiles[index, i].state : tiles[i, index].state;
            if (state == TileState.Filled) filledCount++;
        }

        return filledCount < totalFilled;
    }

    private static bool IsBoardSolved(Tile[,] tiles, Clues clues)
    {
        for (int r = 0; r < tiles.GetLength(0); r++)
        {
            if (!IsRowValid(tiles, clues.RowClues[r], r))
                return false;
        }
        for (int c = 0; c < tiles.GetLength(1); c++)
        {
            if (!IsColumnValid(tiles, clues.ColumnClues[c], c))
                return false;
        }
        return true;
    }

    private static bool IsRowValid(Tile[,] tiles, List<int> clues, int row)
    {
        return CheckLineValidity(tiles, clues, row, true);
    }

    private static bool IsColumnValid(Tile[,] tiles, List<int> clues, int col)
    {
        return CheckLineValidity(tiles, clues, col, false);
    }

    private static bool CheckLineValidity(Tile[,] tiles, List<int> clues, int index, bool isRow)
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

        return foundGroups.SequenceEqual(clues);
    }
}
