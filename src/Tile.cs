using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Serilog;
using System;

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

    private readonly FadeRect _fr;

    public static void LoadTextures(ContentManager content)
    {
        Log.Logger.Information("Loading tile textures");
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
        _fr = new(new(0, 0, 0, 0), Color.Black, Color.Black, 0.13f);
    }

    public void CopyFrom(Tile other)
    {
        // Do a copy without constructing the tile again
        state = other.state;
        isHovered = other.isHovered;
    }

    public readonly void Draw(int x, int y, int boardSize, SpriteBatch batch, GraphicsDevice graphDev)
    {
        Vector2 posVec = getScreenPos(x, y, boardSize, graphDev);
        Rectangle rect = new((int)posVec.X, (int)posVec.Y, 32, 32);
        _fr.rect = rect;

        switch (state)
        {
            default:
                _fr.color1 = Color.Gray;
                _fr.color2 = Color.LightGray;
                break;
            case TileState.Filled:
                _fr.color1 = Color.Green;
                _fr.color2 = Color.Lime;
                break;
        }

        _fr.mode = isHovered ? FadeRectMode.FadeIn : FadeRectMode.FadeOut;
        _fr.Update();

        _fr.Draw(batch);
        if (state == TileState.Cross)
            batch.Draw(TextureCross, posVec, Color.White);
    }

    public void Hover(int x, int y, int mx, int my, int boardSize, GraphicsDevice graphDev)
    {
        Vector2 screenPos = getScreenPos(x, y, boardSize, graphDev);
        Rectangle rect = new((int)screenPos.X, (int)screenPos.Y, 32, 32);
        Point mousePoint = new(mx, my);
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

    public readonly void HintFlash()
    {
        _fr.r = 255;
        _fr.g = 255;
        _fr.b = 0;
    }

    private static Vector2 getScreenPos(int x, int y, int boardSize, GraphicsDevice graphDev)
    {
        int boardPx = boardSize * 32;
        return new Vector2((x * 32) + ((graphDev.Viewport.Bounds.Width / 2) - (boardPx / 2)), (y * 32) + ((graphDev.Viewport.Bounds.Height / 2) - (boardPx / 2)));
    }
}
