using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace NonoSharp;

public class Board
{
    protected Clues? clues;
    protected Stack<Tile[,]> undoStack;
    protected Tile[,]? previousState;
    protected HashSet<(int, int)> hintedLines;

    public Tile[,]? tiles;
    public Tile[,]? solution;
    public int size;
    public int maxHints;
    public bool IsSolved { get; private set; }

    private int _boardX;
    private int _boardY;
    private int _offsetX;
    private int _offsetY;
    private uint _frameCounter;

    public static bool CompareSolutionTile(TileState a, TileState b)
    {
        if (a == TileState.Cross)
            a = TileState.Empty;
        if (b == TileState.Cross)
            b = TileState.Empty;
        return a == b;
    }

    public Board()
    {
        size = 0;
        maxHints = -1;
        undoStack = new Stack<Tile[,]>();
        previousState = null;
        hintedLines = new();
        _boardX = 0;
        _boardY = 0;
        _offsetX = 0;
        _offsetY = 0;
        _frameCounter = 0;
    }

    public Board(int size)
    {
        this.size = size;
        maxHints = -1;
        undoStack = new();
        previousState = null;
        hintedLines = new();
        MakeTilesAndSolution();
        _boardX = 0;
        _boardY = 0;
        _offsetX = 0;
        _offsetY = 0;
        _frameCounter = 0;
    }

    public Board(string fileName)
    {
        undoStack = new();
        previousState = null;
        hintedLines = new();
        Load(fileName);
        _boardX = 0;
        _boardY = 0;
        _offsetX = 0;
        _offsetY = 0;
        _frameCounter = 0;
    }

    public void Load(string fileName)
    {
        Log.Logger.Information($"Loading board from file {fileName}");
        string[] boardData = File.ReadAllLines(fileName);
        int solutionOffset = 2;
        size = int.Parse(boardData[0]);
        if (!int.TryParse(boardData[1], out maxHints))
        {
            maxHints = -1;
            solutionOffset = 1;
        }
        Log.Logger.Information($"Board size: {size}");
        MakeTilesAndSolution();

        if (solution != null)
        {
            for (int i = solutionOffset; i < size + solutionOffset; i++)
            {
                string row = boardData[i];
                for (int j = 0; j < size; j++)
                {
                    char ch = row[j];
                    if (ch == '#')
                        solution[j, i - solutionOffset].state = TileState.Filled;
                    else
                        solution[j, i - solutionOffset].state = TileState.Empty;
                }
            }
        }

        clues = new(this);
        CrossZeroLines();
    }

    public void Draw(SpriteBatch batch)
    {
        GraphicsDevice graphDev = batch.GraphicsDevice;

        if (tiles != null)
        {
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    tiles[i, j].Draw(i, j, _offsetX, _offsetY, size, IsSolved, batch);
        }

        int pxSize = size * 32;
        _boardX = (graphDev.Viewport.Bounds.Width / 2) - (pxSize / 2) + _offsetX;
        _boardY = (graphDev.Viewport.Bounds.Height / 2) - (pxSize / 2) + _offsetY;

        if (IsSolved)
        {
            RectRenderer.DrawRectOutline(getRect(), Color.Black, 1, batch);
        }
        else
        {
            GridRenderer.DrawGrid(batch, _boardX, _boardY, size, size, 32, Color.Black);
            DrawClues(_boardX, _boardY, batch);
        }
    }

    public void Update(MouseState mouseState, MouseState mouseStateOld, KeyboardState kb, KeyboardState kbOld, GraphicsDevice graphDev)
    {
        updateMouseDrag(mouseState, mouseStateOld);

        if (IsSolved)
            return;

        Point mousePoint = new(mouseState.X, mouseState.Y);
        bool canHover = getRect().Contains(mousePoint);

        if (tiles != null)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    ref Tile tile = ref tiles[i, j];
                    if (canHover)
                        tile.Hover(i, j, _offsetX, _offsetY, mouseState.X, mouseState.Y, size, graphDev);
                    else
                    {
                        tile.isHoveredX = false;
                        tile.isHoveredY = false;
                    }
                    if (tile.isHoveredX && tile.isHoveredY)
                    {
                        bool left = (mouseStateOld.LeftButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Pressed) || (kb.IsKeyDown(KeySettings.Get("placeTile")) && !kbOld.IsKeyDown(KeySettings.Get("placeTile")));
                        bool right = (mouseStateOld.RightButton == ButtonState.Released && mouseState.RightButton == ButtonState.Pressed) || (kb.IsKeyDown(Keys.C) && !kbOld.IsKeyDown(Keys.C));

                        if (left || right)
                        {
                            SaveState();
                            CheckSolution();
                            DoMouseInput(left, ref tiles[i, j]);
                        }
                    }
                }
            }
        }


        _frameCounter++;
    }

    public virtual void RestoreState()
    {
        if (tiles == null)
            return;

        if (undoStack.Count > 0)
        {
            previousState = undoStack.Pop();

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    tiles[i, j].CopyFrom(previousState[i, j]);
        }
    }

    public string? Serialize()
    {
        if (tiles == null)
            return null;

        Stopwatch stopwatch = new();
        stopwatch.Start();

        string result = $"{size}\n{maxHints}\n";
        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < size; i++)
            {
                TileState state = tiles[i, j].state;
                if (state == TileState.Empty || state == TileState.Cross)
                    result += ".";
                else
                    result += "#";
            }
            result += "\n";
        }

        stopwatch.Stop();
        Log.Logger.Information($"Serialized board {GetHashCode()} in {stopwatch.ElapsedMilliseconds} ms");

        return result;
    }

    public void Reset()
    {
        solution = null;
        tiles = null;
        IsSolved = false;
        undoStack.Clear();
        hintedLines = new();
    }

    public void SolveLine(int column, int row)
    {
        if (tiles == null || solution == null)
            return;

        Log.Logger.Information($"Solving {column},{row}");
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (i == row || j == column)
                {
                    tiles[j, i].CopyFrom(solution[j, i]);
                    if (tiles[j, i].state == TileState.Empty)
                        tiles[j, i].state = TileState.Cross;
                    tiles[j, i].HintFlash();
                }
            }
        }
    }

    public void Hint()
    {
        SaveState();

        while (true)
        {
            int column = Random.Shared.Next(size);
            int row = Random.Shared.Next(size);

            if (!hintedLines.Contains((column, row)))
            {
                hintedLines.Add((column, row));
                SolveLine(column, row);
                break;
            }
            else
                Log.Logger.Information($"{column},{row} already hinted");
        }
    }

    public void Clear()
    {
        if (tiles == null)
            return;

        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                tiles[i, j].state = TileState.Empty;
    }

    protected void MakeTilesAndSolution()
    {
        tiles = new Tile[size, size];
        solution = new Tile[size, size];

        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                tiles[i, j] = new();
    }

    protected virtual void DrawClues(int boardX, int boardY, SpriteBatch batch)
    {
        if (clues == null)
            return;

        int rowCluesX = boardX - 24;
        int rowCluesY = boardY;

        for (int i = 0; i < size; i++)
            for (int j = 0; j < clues.RowClues[i].Count; j++)
                TextRenderer.DrawText(batch, "DefaultFont", rowCluesX - (j * 24), rowCluesY + (i * 32), 0.5f, clues.RowClues[i][j].ToString(), Color.White);

        int colCluesX = boardX + 12;
        int colCluesY = boardY - 32;
        for (int i = 0; i < size; i++)
            for (int j = 0; j < clues.ColumnClues[i].Count; j++)
                TextRenderer.DrawText(batch, "DefaultFont", colCluesX + (i * 32), colCluesY - (j * 32), 0.5f, clues.ColumnClues[i][j].ToString(), Color.White);
    }

    protected virtual void CheckSolution()
    {
        if (tiles == null || solution == null)
            return;

        bool solved = true;
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
            {
                TileState a = tiles[i, j].state;
                TileState b = solution[i, j].state;
                if (!CompareSolutionTile(a, b))
                {
                    solved = false;
                    goto CheckSolvedEnd;
                }
            }
        CheckSolvedEnd:
        IsSolved = solved;

        if (IsSolved)
        {
            Log.Logger.Information("Board is solved");
            _offsetX = 0;
            _offsetY = 0;
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                {
                    tiles[i, j].isHoveredX = false;
                    tiles[i, j].isHoveredY = false;
                }
        }
    }

    protected virtual void SaveState()
    {
        if (tiles == null)
            return;

        // Create a deep copy of the current board state
        Tile[,] currentState = new Tile[size, size];
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                currentState[i, j].CopyFrom(tiles[i, j]);

        undoStack.Push(currentState);
    }

    protected virtual void DoMouseInput(bool isLeftButton, ref Tile tile)
    {
        if (isLeftButton)
            tile.LeftClick();
        else
            tile.RightClick();
    }

    public void CrossZeroLines()
    {
        // Find all lines that have no tiles and cross them
        // (if solution and tiles aren't null of course)

        if (tiles == null || solution == null)
            return;

        for (int i = 0; i < size; i++)
        {
            bool allZeros = true;
            for (int j = 0; j < size; j++)
            {
                if (solution[j, i].state != TileState.Empty)
                {
                    allZeros = false;
                    break;
                }
            }

            if (allZeros)
            {
                // put crosses
                for (int j = 0; j < size; j++)
                    tiles[j, i].state = TileState.Cross;
            }

            // do the same in the other direction
            allZeros = true;
            for (int j = 0; j < size; j++)
            {
                if (solution[i, j].state != TileState.Empty)
                {
                    allZeros = false;
                    break;
                }
            }

            if (allZeros)
            {
                // put crosses
                for (int j = 0; j < size; j++)
                    tiles[i, j].state = TileState.Cross;
            }
        }
    }

    private Rectangle getRect()
    {
        int pxSize = size * 32;
        return new(_boardX, _boardY, pxSize, pxSize);
    }

    private void updateMouseDrag(MouseState mouseState, MouseState mouseStateOld)
    {
        if (mouseState.MiddleButton == ButtonState.Pressed)
        {
            int deltaX = mouseState.X - mouseStateOld.X;
            int deltaY = mouseState.Y - mouseStateOld.Y;
            _offsetX += deltaX;
            _offsetY += deltaY;
        }
    }
}
