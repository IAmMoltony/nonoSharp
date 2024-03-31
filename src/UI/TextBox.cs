using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace NonoSharp.UI;

public class TextBox : UIElement
{
    public static readonly int Height = 30;

    public int Width { get; private set; }
    public string Text { get; private set; }
    public bool Hovered { get; private set; }

    public List<char> illegalChars;
    public int maxLength;

    private Color _fillColor;
    private Color _outlineColor;
    private Color _textColor;
    private Color _textColorHover;
    private bool _blinkCursor;
    private int _blinkCursorTimer;

    public TextBox(
        int x, int y, int width, Color fillColor, Color outlineColor, Color textColor,
        Color textColorHover, int maxLength = 0) : base(x, y)
    {
        Width = width;
        Text = "";
        Hovered = false;
        illegalChars = new();
        _fillColor = fillColor;
        _outlineColor = outlineColor;
        _textColor = textColor;
        _textColorHover = textColorHover;
        this.maxLength = maxLength;
    }

    public override void Draw(SpriteBatch sprBatch)
    {
        Color fc = Hovered ? _outlineColor : _fillColor;
        Color oc = Hovered ? _fillColor : _outlineColor;
        RectRenderer.DrawRect(getRect(), fc, sprBatch);
        RectRenderer.DrawRectOutline(getRect(), oc, 2, sprBatch);

        drawText(sprBatch);
    }

    public override void Update(MouseState mouse, MouseState mouseOld, KeyboardState keyboard, KeyboardState keyboardOld)
    {
        checkHovered(mouse);
        updateBlinkCursor();

        if (Hovered)
            NonoSharpGame.Cursor = MouseCursor.IBeam;
    }

    public virtual void UpdateInput(object sender, TextInputEventArgs tiea)
    {
        if (Hovered)
        {
            switch (tiea.Key)
            {
                // Backspace: remove char
                case Keys.Back:
                    BackSpace();
                    break;
                default:
                    if (checkLength() && !illegalChars.Contains(tiea.Character))
                        Text += tiea.Character;
                    break;
            }
        }
    }

    public void BackSpace()
    {
        if (Text.Length > 0)
            Text = Text.Substring(0, Text.Length - 1);
    }

    public void Clear()
    {
        Text = "";
    }

    private Rectangle getRect()
    {
        return new(x, y, Width, Height);
    }

    private void drawText(SpriteBatch sprBatch)
    {
        Color textColor = Hovered ? _textColorHover : _textColor;
        Vector2 textSize = TextRenderer.MeasureString("DefaultFont", Text);
        float textWidth = textSize.X * 0.5f;
        float maxTextWidth = Width - 17;
        float offset = 0;

        if (textWidth > maxTextWidth)
            offset = textWidth - maxTextWidth;

        sprBatch.End();

        RasterizerState rs = new()
        {
            ScissorTestEnable = true
        };
        sprBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, rs);
        sprBatch.GraphicsDevice.ScissorRectangle = getRect();
        TextRenderer.DrawText(sprBatch, "DefaultFont", x + 4 - (int)offset, y, 0.5f, Text + ((_blinkCursor && Hovered) ? "_" : ""), textColor);
        sprBatch.End();

        sprBatch.Begin();
    }

    private void updateBlinkCursor()
    {
        _blinkCursorTimer++;
        if (_blinkCursorTimer % 40 == 0)
            _blinkCursor = !_blinkCursor;
    }

    private void checkHovered(MouseState mouse)
    {
        Rectangle rect = getRect();
        Hovered = mouse.X >= rect.X && mouse.Y >= rect.Y && mouse.X <= (rect.X + rect.Width) && mouse.Y <= (rect.Y + rect.Height);
    }

    private bool checkLength()
    {
        return maxLength == 0 || (Text.Length + 1) < maxLength;
    }
}
