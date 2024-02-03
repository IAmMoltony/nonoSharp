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
    public TileState State { get; set; }
    public bool IsHovered;

    public static Texture2D TextureCross { get; private set; }

    public static void LoadTextures(ContentManager content)
    {
        TextureCross = content.Load<Texture2D>("cross");
    }

    public Tile()
    {
        State = TileState.Empty;
        IsHovered = false;
    }

    public void Draw(int x, int y, int boardSize, SpriteBatch batch, GraphicsDevice graphDev)
    {
        Vector2 posVec = getScreenPos(x, y, boardSize, graphDev);
        Rectangle rect = new Rectangle((int)posVec.X, (int)posVec.Y, 32, 32);

        Color color = Color.Gray;
        if (IsHovered)
        {
            switch (State)
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
            switch (State)
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
        if (State == TileState.Cross)
            batch.Draw(TextureCross, posVec, Color.White);
    }

    public void Hover(int x, int y, int mx, int my, int boardSize, GraphicsDevice graphDev)
    {
        Vector2 screenPos = getScreenPos(x, y, boardSize, graphDev);
        Rectangle rect = new Rectangle((int)screenPos.X, (int)screenPos.Y, 32, 32);
        Point mousePoint = new Point(mx, my);
        IsHovered = rect.Contains(mousePoint);
    }

    public void LeftClick()
    {
        if (State == TileState.Empty)
            State = TileState.Filled;
        else if (State == TileState.Filled)
            State = TileState.Empty;
    }

    public void RightClick()
    {
        if (State == TileState.Empty)
            State = TileState.Cross;
        else if (State == TileState.Cross)
            State = TileState.Empty;
    }

    private Vector2 getScreenPos(int x, int y, int boardSize, GraphicsDevice graphDev)
    {
        int boardPx = boardSize * 32;
        return new Vector2(x * 32 + (graphDev.Viewport.Bounds.Width / 2 - boardPx / 2), y * 32 + (graphDev.Viewport.Bounds.Height / 2 - boardPx / 2));
    }
}
