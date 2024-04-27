using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace NonoSharp.UI;

public class CheckBox : UIElement
{
    public static readonly int Size = 26;

    public bool isChecked;

    private Color _fillColor, _outlineColor;
    private bool _isHovered;

    public CheckBox(int x, int y, Color fillColor, Color outlineColor) : base(x, y)
    {
        isChecked = false;
        _fillColor = fillColor;
        _outlineColor = outlineColor;
        _isHovered = false;
    }

    public override void Draw(SpriteBatch sprBatch)
    {
        Rectangle rect = getRect();
        Color fc = _isHovered ? _outlineColor : _fillColor;
        Color oc = _isHovered ? _fillColor : _outlineColor;
        RectRenderer.DrawRect(rect, fc, sprBatch);
        RectRenderer.DrawRectOutline(rect, oc, 2, sprBatch);
    }

    public override void Update(MouseState mouse, MouseState mouseOld, KeyboardState keyboard, KeyboardState keyboardOld)
    {
        _isHovered = mouse.X > x && mouse.Y > y && mouse.X < x + Size && mouse.Y < y + Size;

        if (_isHovered && mouse.LeftButton == ButtonState.Pressed && mouseOld.LeftButton == ButtonState.Released)
            isChecked = !isChecked;
    }

    private Rectangle getRect()
    {
        return new(x, y, Size, Size);
    }
}
