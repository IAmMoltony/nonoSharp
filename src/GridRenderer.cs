using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Serilog;

namespace NonoSharp;

public class GridRenderer
{
    private static Texture2D _texture;

    private GridRenderer()
    {
    }

    public static void Load(GraphicsDevice graphDev)
    {
        Log.Logger.Information("Loading grid renderer");
        _texture = new(graphDev, 1, 1);
        _texture.SetData(new Color[] { Color.White });
    }

    public static void DrawGrid(SpriteBatch batch, int x, int y, int rows, int cols, int gridSize, Color color)
    {
        for (int i = 0; i < cols; i++)
            batch.Draw(_texture, new Rectangle(x + i * gridSize, y, 1, rows * gridSize), color);
        for (int i = 0; i < rows; i++)
            batch.Draw(_texture, new Rectangle(x, y + i * gridSize, cols * gridSize, 1), color);
    }
}
