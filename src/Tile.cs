using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace NonoSharp;

public enum TileState
{
    Empty,
    Filled,
    Cross
}

public struct Tile
{
    public TileState state;
    public bool isHovered;

    public static Texture2D TextureCross { get; private set; }

    public static void LoadTextures(ContentManager content)
    {
        TextureCross = content.Load<Texture2D>("cross");
    }

    public static void PrintTileArray(Tile[,] ta, int size)
    {
        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < size; i++)
                switch (ta[i, j].state)
                {
                    case TileState.Empty:
                        Console.Write(" ");
                        break;
                    case TileState.Filled:
                        Console.Write("#");
                        break;
                    case TileState.Cross:
                        Console.Write("X");
                        break;
                }
            Console.Write("\n");
        }
    }

    public Tile()
    {
        state = TileState.Empty;
        isHovered = false;
    }

    public void Draw(int x, int y, int boardSize, SpriteBatch batch, GraphicsDevice graphDev)
    {
        Vector2 posVec = getScreenPos(x, y, boardSize, graphDev);
        Rectangle rect = new Rectangle((int)posVec.X, (int)posVec.Y, 32, 32);

        Color color = Color.Gray;
        if (isHovered)
        {
            switch (state)
            {
                default:
                    color = Color.LightGray;
                    break;
                case TileState.Filled:
                    color = Color.Lime;
                    break;
            }
        }
        else
        {
            switch (state)
            {
                default:
                    color = Color.Gray;
                    break;
                case TileState.Filled:
                    color = Color.Green;
                    break;
            }
        }

        RectRenderer.DrawRect(rect, color, batch);
        if (state == TileState.Cross)
            batch.Draw(TextureCross, posVec, Color.White);
    }

    public void Hover(int x, int y, int mx, int my, int boardSize, GraphicsDevice graphDev)
    {
        Vector2 screenPos = getScreenPos(x, y, boardSize, graphDev);
        Rectangle rect = new Rectangle((int)screenPos.X, (int)screenPos.Y, 32, 32);
        Point mousePoint = new Point(mx, my);
        isHovered = rect.Contains(mousePoint);
    }

    public void LeftClick()
    {
        if (state == TileState.Empty)
            state = TileState.Filled;
        else if (state == TileState.Filled)
            state = TileState.Empty;
    }

    public void RightClick()
    {
        if (state == TileState.Empty)
            state = TileState.Cross;
        else if (state == TileState.Cross)
            state = TileState.Empty;
    }

    private Vector2 getScreenPos(int x, int y, int boardSize, GraphicsDevice graphDev)
    {
        int boardPx = boardSize * 32;
        return new Vector2(x * 32 + (graphDev.Viewport.Bounds.Width / 2 - boardPx / 2), y * 32 + (graphDev.Viewport.Bounds.Height / 2 - boardPx / 2));
    }
}
