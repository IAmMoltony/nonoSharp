using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;

namespace NonoSharp;

public class Board
{
    public Tile[,] tiles;
    public Tile[,] solution;
    public int size;
    public bool IsSolved { get; private set; }

    public Board()
    {
        this.size = 0;
    }

    public Board(int size)
    {
        this.size = size;
        makeTilesAndSolution();
    }

    public Board(string fileName)
    {
        Load(fileName);
    }

    public void Load(string fileName)
    {
        string[] boardData = File.ReadAllLines(fileName);        
        size = int.Parse(boardData[0]);
        makeTilesAndSolution();

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
    }

    public void Draw(SpriteBatch batch, GraphicsDevice graphDev)
    {
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                tiles[i, j].Draw(i, j, size, batch, graphDev);
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
                    if (mouseStateOld.RightButton == ButtonState.Released && mouseState.RightButton == ButtonState.Pressed)
                        tile.RightClick();
                }
            }
        checkSolved();
    }

    private void makeTilesAndSolution()
    {
        tiles = new Tile[size, size];
        solution = new Tile[size, size];
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
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    tiles[i, j].isHovered = false;
        }
     }
}
