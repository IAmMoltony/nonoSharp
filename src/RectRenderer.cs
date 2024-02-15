using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Serilog;

namespace NonoSharp;

public class RectRenderer
{
    private static Texture2D _texture;

    private RectRenderer()
    {
    }

    public static void Load(GraphicsDevice graphDev)
    {
        Log.Logger.Information("Loading rect renderer");
        _texture = new Texture2D(graphDev, 1, 1);
        _texture.SetData(new[] { Color.White });
    }

    public static void DrawRect(Rectangle rect, Color color, SpriteBatch batch)
    {
        batch.Draw(_texture, rect, color);
    }

    public static void DrawRectOutline(Rectangle rect, Color color, int thickness, SpriteBatch batch)
    {
        DrawRect(new Rectangle(rect.X, rect.Y, thickness, rect.Height + thickness), color, batch);
        DrawRect(new Rectangle(rect.X, rect.Y, rect.Width + thickness, thickness), color, batch);
        DrawRect(new Rectangle(rect.X + rect.Width, rect.Y, thickness, rect.Height + thickness), color, batch);
        DrawRect(new Rectangle(rect.X, rect.Y + rect.Height, rect.Width + thickness, thickness), color, batch);
    }
}
