using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
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
    public bool isHoveredX;
    public bool isHoveredY;

    public static Texture2D TextureCross { get; private set; }
    public static SoundEffect SoundPlace { get; private set; }
    public static SoundEffect SoundCross { get; private set; }

    private readonly FadeRect _fr;

    public static void LoadTextures(ContentManager content)
    {
        Log.Logger.Information("Loading tile textures");
        TextureCross = content.Load<Texture2D>("image/cross");
    }

    public static void LoadSounds(ContentManager content)
    {
        Log.Logger.Information("Loading tile sounds");
        SoundPlace = content.Load<SoundEffect>("sound/tilePlace");
        SoundCross = content.Load<SoundEffect>("sound/cross");
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
        isHoveredX = false;
        isHoveredY = false;
        _fr = new(new(0, 0, 0, 0), Color.Black, Color.Black, 0.13f);
    }

    public void CopyFrom(Tile other)
    {
        // Do a copy without constructing the tile again
        state = other.state;
        isHoveredX = other.isHoveredX;
        isHoveredY = other.isHoveredY;
    }

    public readonly void Draw(int x, int y, int boardSize, bool isBoardSolved, SpriteBatch batch)
    {
        GraphicsDevice graphDev = batch.GraphicsDevice;

        Vector2 posVec = getScreenPos(x, y, boardSize, graphDev);
        Rectangle rect = new((int)posVec.X, (int)posVec.Y, 32, 32);
        _fr.rect = rect;

        switch (state)
        {
            default:
                _fr.color1 = Color.Gray;
                _fr.color2 = (isHoveredX ^ isHoveredY) ? Color.Gray.Lighter(0.16f) : Color.LightGray;
                break;
            case TileState.Filled:
                _fr.color1 = Settings.GetAccentColor();
                _fr.color2 = Settings.GetLightAccentColor(0.16f);
                break;
        }

        _fr.mode = (isHoveredX || isHoveredY) ? FadeRectMode.FadeIn : FadeRectMode.FadeOut;
        _fr.Update();

        _fr.Draw(batch);
        if (state == TileState.Cross && !isBoardSolved)
            batch.Draw(TextureCross, posVec, Color.White);
    }

    public void Hover(int x, int y, int mx, int my, int boardSize, GraphicsDevice graphDev)
    {
        Vector2 screenPos = getScreenPos(x, y, boardSize, graphDev);
        Rectangle rect = new((int)screenPos.X, (int)screenPos.Y, 32, 32);
        isHoveredX = mx > rect.X && mx < rect.X + rect.Width;
        isHoveredY = my > rect.Y && my < rect.Y + rect.Height;
    }

    public void LeftClick()
    {
        if (state == TileState.Empty)
        {
            if (Settings.GetBool("sound"))
                SoundPlace.Play();
            state = TileState.Filled;
        }
        else if (state == TileState.Filled)
            state = TileState.Empty;
    }

    public void RightClick()
    {
        if (state == TileState.Empty)
        {
            if (Settings.GetBool("sound"))
                SoundCross.Play();
            state = TileState.Cross;
        }
        else if (state == TileState.Cross)
            state = TileState.Empty;
    }

    public readonly void HintFlash()
    {
        Color accentColor = Settings.GetAccentColor();
        Color flashColor = new(((int)accentColor.R + 255) % 256, ((int)accentColor.G + 127) % 256, ((int)accentColor.B - 255) % 256);
        _fr.SetColor(flashColor);
    }

    private static Vector2 getScreenPos(int x, int y, int boardSize, GraphicsDevice graphDev)
    {
        int boardPx = boardSize * 32;
        return new Vector2((x * 32) + ((graphDev.Viewport.Bounds.Width / 2) - (boardPx / 2)), (y * 32) + ((graphDev.Viewport.Bounds.Height / 2) - (boardPx / 2)));
    }
}
