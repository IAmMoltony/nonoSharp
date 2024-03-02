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
    private Color _textColor;
    private Color _textColorHover;
    private bool _blinkCursor;
    private int _blinkCursorTimer;

    public TextBox(
        int x, int y, int width, Color fillColor, Color outlineColor, Color textColor,
        Color textColorHover) : base(x, y)
    {
        Width = width;
        Text = "";
        Hovered = false;
        _fillColor = fillColor;
        _outlineColor = outlineColor;
        _textColor = textColor;
        _textColorHover = textColorHover;
    }

    public override void Draw(SpriteBatch sprBatch)
    {
        Color fc = Hovered ? _outlineColor : _fillColor;
        Color oc = Hovered ? _fillColor : _outlineColor;
        Color tc = Hovered ? _textColorHover : _textColor;
        RectRenderer.DrawRect(getRect(), fc, sprBatch);
        RectRenderer.DrawRectOutline(getRect(), oc, 2, sprBatch);

        Vector2 textSize = TextRenderer.MeasureString("notosans", Text);
        float textWidth = textSize.X * 0.5f;
        float maxTextWidth = Width - 17;  // Adjust this as needed
        float offset = 0;

        if (textWidth > maxTextWidth)
        {
            offset = textWidth - maxTextWidth;
        }

        TextRenderer.DrawText(sprBatch, "notosans", x + 4 - (int)offset, y, 0.5f, Text + ((_blinkCursor && Hovered) ? "_" : ""), tc);
    }

    public override void Update(MouseState mouse, MouseState mouseOld, KeyboardState keyboard, KeyboardState keyboardOld)
    {
        Rectangle rect = getRect();
        Hovered = mouse.X >= rect.X && mouse.Y >= rect.Y && mouse.X <= (rect.X + rect.Width) && mouse.Y <= (rect.Y + rect.Height);

        _blinkCursorTimer++;
        if (_blinkCursorTimer % 40 == 0)
            _blinkCursor = !_blinkCursor;
    }

    public void UpdateInput(object sender, TextInputEventArgs tiea)
    {
        if (Hovered)
        {
            switch (tiea.Key)
            {
            // Backspace: remove char
            case Keys.Back:
                if (Text.Length > 0)
                    Text = Text.Substring(0, Text.Length - 1);
                break;
            default:
                Text += tiea.Character;
                break;
            }
        }
    }

    private Rectangle getRect()
    {
        return new(x, y, Width, 30);
    }
}
