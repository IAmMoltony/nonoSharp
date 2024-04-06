using System;
using System.Collections.Generic;
using System.Diagnostics;
using Serilog;

namespace NonoSharp;

public class Clues
{
    public List<int>[] RowClues { get; private set; }
    public List<int>[] ColumnClues { get; private set; }

    public Clues(int size)
    {
        RowClues = new List<int>[size];
        ColumnClues = new List<int>[size];

        for (int i = 0; i < size; i++)
        {
            RowClues[i] = new();
            ColumnClues[i] = new();
        }
    }

    public Clues(Board board, bool useSolution = true)
    {
        Log.Logger.Information("Creating clues");

        RowClues = new List<int>[board.size];
        ColumnClues = new List<int>[board.size];

        // initialize lists
        for (int i = 0; i < board.size; i++)
        {
            RowClues[i] = new();
            ColumnClues[i] = new();
        }

        Stopwatch stopwatch = new();
        stopwatch.Start();

        ref Tile[,] tiles = ref board.solution;
        if (!useSolution)
            tiles = ref board.tiles;

        Log.Logger.Information("Finding row clues...");

        // find row clues
        for (int row = 0; row < board.size; row++)
        {
            // copy row into an array
            Tile[] rowTiles = new Tile[board.size];
            for (int i = 0; i < board.size; i++)
                rowTiles[i] = tiles[i, row];

            int counter = 0;
            foreach (Tile tile in rowTiles)
            {
                if (tile.state == TileState.Filled)
                    counter++;
                if (tile.state == TileState.Empty && counter > 0)
                {
                    RowClues[row].Add(counter);
                    counter = 0;
                }
            }

            if (RowClues[row].Count == 0 || counter != 0)
                RowClues[row].Add(counter);
            RowClues[row].Reverse();
        }

        Log.Logger.Information("Finding column clues...");

        // find column clues
        for (int col = 0; col < board.size; col++)
        {
            // copy column into an array
            Tile[] colTiles = new Tile[board.size];
            for (int i = 0; i < board.size; i++)
                colTiles[i] = tiles[col, i];

            int counter = 0;
            foreach (Tile tile in colTiles)
            {
                if (tile.state == TileState.Filled)
                    counter++;
                if (tile.state == TileState.Empty && counter > 0)
                {
                    ColumnClues[col].Add(counter);
                    counter = 0;
                }
            }

            if (ColumnClues[col].Count == 0 || counter != 0)
                ColumnClues[col].Add(counter);
            ColumnClues[col].Reverse();
        }

        stopwatch.Stop();

        Log.Logger.Information($"Clues found in {stopwatch.ElapsedMilliseconds} ms");
    }
}
