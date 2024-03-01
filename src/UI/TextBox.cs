using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace NonoSharp.UI;

public class TextBox : UIElement
{
    public int Width { get; private set; }
    public string Text { get; private set; }
    public bool Hovered { get; private set; }

    private Color _fillColor;
    private Color _outlineColor;

    public TextBox(int x, int y, int width, Color fillColor, Color outlineColor) : base(x, y)
    {
        Width = width;
        Text = "";
        Hovered = false;
        _fillColor = fillColor;
        _outlineColor = outlineColor;
    }

    public override void Draw(SpriteBatch sprBatch)
    {
        Color fc = Hovered ? _outlineColor : _fillColor;
        Color oc = Hovered ? _fillColor : _outlineColor;
        RectRenderer.DrawRect(getRect(), fc, sprBatch);
        RectRenderer.DrawRectOutline(getRect(), oc, 2, sprBatch);
    }

    public override void Update(MouseState mouse, MouseState mouseOld, KeyboardState keyboard, KeyboardState keyboardOld)
    {
        Rectangle rect = getRect();
        Hovered = mouse.X >= rect.X && mouse.Y >= rect.Y && mouse.X <= (rect.X + rect.Width) && mouse.Y <= (rect.Y + rect.Height);
    }

    private Rectangle getRect()
    {
        return new(x, y, Width, 30);
    }
}
