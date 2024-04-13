using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace NonoSharp;

public class MenuBackgroundTile
{
    public int x, y;
    public TileState State { get; private set; }

    private TileState? _oldState;

    public MenuBackgroundTile(int x, int y)
    {
        this.x = x;
        this.y = y;
        _oldState = null;
        RandomizeState();
    }

    public void RandomizeState()
    {
        do
        {
            State = (TileState)Random.Shared.Next(0, 3);
        } while (_oldState == State);
        _oldState = State;
    }

    public void Draw(SpriteBatch sprBatch)
    {
        Rectangle rect = new(x, y, 32, 32);

        switch (State)
        {
            case TileState.Empty:
                RectRenderer.DrawRect(rect, Color.DarkGray.Darker(0.6f), sprBatch);
                break;
            case TileState.Filled:
                RectRenderer.DrawRect(rect, Color.DarkGreen.Darker(0.6f), sprBatch);
                break;
            case TileState.Cross:
                RectRenderer.DrawRect(rect, Color.DarkGray.Darker(0.6f), sprBatch);
                sprBatch.Draw(Tile.TextureCross, new Vector2(x, y), Color.White);
                break;
        }
    }

    public void Update()
    {
        x--;
        y--;
    }
}
