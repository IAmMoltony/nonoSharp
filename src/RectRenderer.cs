using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NonoSharp;

public class RectRenderer
{
    private static Texture2D _texture;

    private RectRenderer()
    {
    }

    public static void Load(GraphicsDevice graphDev)
    {
        _texture = new Texture2D(graphDev, 1, 1);
        _texture.SetData(new[] { Color.White });
    }

    public static void DrawRect(Rectangle rect, Color color, SpriteBatch batch)
    {
        batch.Draw(_texture, rect, color);
    }
}
