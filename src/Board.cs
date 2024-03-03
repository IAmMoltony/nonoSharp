using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using Serilog;

namespace NonoSharp;

public class Board
{
    protected Clues clues;
    protected bool drawClues;
    protected bool checkSolution;
    protected bool canRightClick;

    public Tile[,] tiles;
    public Tile[,] solution;
    public int size;
    public bool IsSolved { get; private set; }

    public Board()
    {
        size = 0;
        drawClues = true;
        checkSolution = true;
        canRightClick = true;
    }

    public Board(int size)
    {
        this.size = size;
        drawClues = true;
        checkSolution = true;
        canRightClick = true;
        MakeTilesAndSolution();
    }

    public Board(string fileName)
    {
        drawClues = true;
        checkSolution = true;
        canRightClick = true;
        Load(fileName);
    }

    public void Load(string fileName)
    {
        Log.Logger.Information($"Loading board from file {fileName}");
        string[] boardData = File.ReadAllLines(fileName);
        size = int.Parse(boardData[0]);
        Log.Logger.Information($"Board size: {size}");
        MakeTilesAndSolution();

        for (int i = 1; i < boardData.Length; i++)
        {
            string row = boardData[i];
            for (int j = 0; j < size; j++)
            {
                char ch = row[j];
                if (ch == '#')
                    solution[j, i - 1].state = TileState.Filled;
                else
                    solution[j, i - 1].state = TileState.Empty;
            }
        }

        clues = new(this);
    }

    public void Draw(SpriteBatch batch)
    {
        GraphicsDevice graphDev = batch.GraphicsDevice;

        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                tiles[i, j].Draw(i, j, size, batch, graphDev);

        int pxSize = size * 32;
        int boardX = graphDev.Viewport.Bounds.Width / 2 - pxSize / 2;
        int boardY = graphDev.Viewport.Bounds.Height / 2 - pxSize / 2;

        if (drawClues)
        {
            int rowCluesX = boardX - 24;
            int rowCluesY = boardY;

            for (int i = 0; i < size; i++)
                for (int j = 0; j < clues.RowClues[i].Count; j++)
                    TextRenderer.DrawText(batch, "notosans", rowCluesX - j * 24, rowCluesY + i * 32, 0.5f, clues.RowClues[i][j].ToString(), Color.White);

            int colCluesX = boardX + 12;
            int colCluesY = boardY - 32;
            for (int i = 0; i < size; i++)
                for (int j = 0; j < clues.ColumnClues[i].Count; j++)
                    TextRenderer.DrawText(batch, "notosans", colCluesX + i * 32, colCluesY - j * 32, 0.5f, clues.ColumnClues[i][j].ToString(), Color.White);
        }

        if (!IsSolved)
            GridRenderer.DrawGrid(batch, boardX, boardY, size, size, 32, Color.Black);
    }

    public void Update(MouseState mouseState, MouseState mouseStateOld, GraphicsDevice graphDev)
    {
        if (IsSolved)
            return;
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
            {
                ref Tile tile = ref tiles[i, j];
                tile.Hover(i, j, mouseState.X, mouseState.Y, size, graphDev);
                if (tile.isHovered)
                {
                    if (mouseStateOld.LeftButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Pressed)
                        tile.LeftClick();
                    if (
                        canRightClick &&
                        mouseStateOld.RightButton == ButtonState.Released &&
                        mouseState.RightButton == ButtonState.Pressed)
                        tile.RightClick();
                }
            }
        checkSolved();
    }

    protected void MakeTilesAndSolution()
    {
        tiles = new Tile[size, size];
        solution = new Tile[size, size];

        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                tiles[i, j] = new();
    }

    private bool compareSolutionTile(TileState a, TileState b)
    {
        if (a == TileState.Cross)
            a = TileState.Empty;
        if (b == TileState.Cross)
            b = TileState.Empty;
        return a == b;
    }

    private void checkSolved()
    {
        if (!checkSolution)
            return;

        bool solved = true;
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
            {
                TileState a = tiles[i, j].state;
                TileState b = solution[i, j].state;
                if (!compareSolutionTile(a, b))
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
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    tiles[i, j].isHovered = false;
        }
    }
}
