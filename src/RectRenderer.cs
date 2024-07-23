using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Serilog;

namespace NonoSharp;

public static class RectRenderer
{
    /// <summary>
    /// 1x1 pixel texture used for rendering rectangles
    /// </summary>
    private static Texture2D? _texture;

    /// <summary>
    /// Load the rectangle renderer
    /// </summary>
    /// <param name="graphDev">Graphics device to use</param>
    public static void Load(GraphicsDevice graphDev)
    {
        Log.Logger.Information("Loading rect renderer");
        _texture = new Texture2D(graphDev, 1, 1);
        _texture.SetData(new[] { Color.White });
    }

    /// <summary>
    /// Draw a filled rectangle
    /// </summary>
    /// <param name="rect">Rectangle to draw</param>
    /// <param name="color">Color of the rectangle</param>
    /// <param name="batch">Sprite batch to use</param>
    public static void DrawRect(Rectangle rect, Color color, SpriteBatch batch)
    {
        if (_texture == null)
            return; // If texture is null then we can't draw a rectangle

        batch.Draw(_texture, rect, color);
    }

    /// <summary>
    /// Draw a rectangle outline
    /// </summary>
    /// <param name="rect">Rectangle to draw</param>
    /// <param name="color">Color of the outline</param>
    /// <param name="thickness">How thick the outline should be (in pixels)</param>
    /// <param name="batch">Sprite batch to use</param>
    public static void DrawRectOutline(Rectangle rect, Color color, int thickness, SpriteBatch batch)
    {
        DrawRect(new Rectangle(rect.X, rect.Y, thickness, rect.Height + thickness), color, batch);
        DrawRect(new Rectangle(rect.X, rect.Y, rect.Width + thickness, thickness), color, batch);
        DrawRect(new Rectangle(rect.X + rect.Width, rect.Y, thickness, rect.Height + thickness), color, batch);
        DrawRect(new Rectangle(rect.X, rect.Y + rect.Height, rect.Width + thickness, thickness), color, batch);
    }

    /// <summary>
    /// Draw a rectangle outline with 1 pixel thickness
    /// </summary>
    /// <param name="rect">Rectangle to draw</param>
    /// <param name="color">Color of the outline</param>
    /// <param name="sprBatch">Sprite batch to use</param>
    public static void DrawRectOutline(Rectangle rect, Color color, SpriteBatch sprBatch)
    {
        DrawRectOutline(rect, color, 1, sprBatch);
    }
}
