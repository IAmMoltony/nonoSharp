using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NonoSharp.UI;

namespace NonoSharp.UI;

public class Dialog
{
    public Rectangle rect;
    private string _title;
    private Color _fillColor;
    private Color _outlineColor;
    private bool _dim;

    public Dialog(int width, int height, string title, Color fillColor, Color outlineColor, bool dim = true)
    {
        rect = new(0, 0, width, height);
        _title = title;
        _fillColor = fillColor;
        _outlineColor = outlineColor;
        _dim = dim;
    }

    public void Draw(SpriteBatch sprBatch)
    {
        GraphicsDevice graphDev = sprBatch.GraphicsDevice;

        if (_dim)
        {
            RectRenderer.DrawRect(new(0, 0, graphDev.Viewport.Bounds.Width, graphDev.Viewport.Bounds.Height), new(Color.Black, 0.5f), sprBatch);
        }

        Rectangle textRect = new();
        textRect = rect;
        textRect.Y -= 100;

        RectRenderer.DrawRect(rect, _fillColor, sprBatch);
        RectRenderer.DrawRectOutline(rect, _outlineColor, 2, sprBatch);

        TextRenderer.DrawTextCenter(sprBatch, "DefaultFont", 0.8f, _title, Color.White, textRect);
    }

    public void Update(GraphicsDevice graphDev)
    {
        rect.X = (graphDev.Viewport.Bounds.Width / 2) - (rect.Width / 2);
        rect.Y = (graphDev.Viewport.Bounds.Height / 2) - (rect.Height / 2);
    }

    public void SetTitle(string title)
    {
        _title = title;
    }
}
