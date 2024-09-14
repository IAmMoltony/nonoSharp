using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace NonoSharp.UI;

public class TextBox : UIElement
{
    public static readonly int Height = 30;

    public string text { get; private set; }
    public bool Hovered { get; private set; }

    public int width;
    public List<char> illegalChars;
    public int maxLength;

    private Color _fillColor;
    private Color _outlineColor;
    private Color _textColor;
    private Color _textColorHover;
    private Color _placeholderColor;
    private Color _placeholderColorHover;
    private readonly string _placeholder;
    private bool _blinkCursor;
    private int _blinkCursorTimer;

    protected bool illegalBlink;

    public TextBox(
        int x, int y, int width, Color fillColor, Color outlineColor, Color textColor,
        Color textColorHover, int maxLength = 0) : base(x, y)
    {
        this.width = width;
        text = "";
        Hovered = false;
        illegalChars = new();
        _fillColor = fillColor;
        _outlineColor = outlineColor;
        _textColor = textColor;
        _textColorHover = textColorHover;
        illegalBlink = false;
        this.maxLength = maxLength;
        _placeholder = "";
    }

    public TextBox(
        int x, int y, int width, Color fillColor, Color outlineColor, Color textColor,
        Color textColorHover, Color placeholderColor, Color placeholderColorHover, string placeholder, int maxLength = 0)
        : this(x, y, width, fillColor, outlineColor, textColor, textColorHover, maxLength)
    {
        _placeholderColor = placeholderColor;
        _placeholderColorHover = placeholderColorHover;
        _placeholder = placeholder;
    }

    public override void Draw(SpriteBatch sprBatch)
    {
        Color fc = Hovered ? _outlineColor : _fillColor;
        Color oc = Hovered ? _fillColor : _outlineColor;
        if (illegalBlink)
        {
            (fc, oc) = (oc, fc); // Python moment
            fc = fc.Lighter(0.5f);
            oc = oc.Lighter(0.5f);
        }
        RectRenderer.DrawRect(getRect(), fc, sprBatch);
        RectRenderer.DrawRectOutline(getRect(), oc, 2, sprBatch);

        drawText(sprBatch);

        illegalBlink = false;
    }

    public override void Update(MouseState mouse, MouseState mouseOld, KeyboardState keyboard, KeyboardState keyboardOld)
    {
        checkHovered(mouse);
        updateBlinkCursor();

        if (Hovered)
            NonoSharpGame.Cursor = MouseCursor.IBeam;
    }

    public virtual void UpdateInput(TextInputEventArgs tiea)
    {
        if (Hovered)
        {
            switch (tiea.Key)
            {
                // Backspace: remove char
                case Keys.Back:
                    if (!BackSpace())
                        illegalBlink = true;
                    break;
                default:
                    if (checkLength())
                        if (illegalChars.Contains(tiea.Character))
                            illegalBlink = true;
                        else
                            text += tiea.Character;
                    else
                        illegalBlink = true;
                    break;
            }
        }
    }

    public bool BackSpace()
    {
        if (text.Length > 0)
        {
            text = text[..^1];
            return true;
        }
        return false;
    }

    public void Clear()
    {
        text = "";
    }

    private Rectangle getRect()
    {
        return new(x, y, width, Height);
    }

    private void drawText(SpriteBatch sprBatch)
    {
        Color textColor = Hovered ? _textColorHover : _textColor;
        Color placeholderColor = Hovered ? _placeholderColorHover : _placeholderColor;
        Vector2 textSize = TextRenderer.MeasureString("DefaultFont", text);
        float textWidth = textSize.X * 0.5f;
        float maxTextWidth = width - 17;
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
        if (string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(_placeholder))
            TextRenderer.DrawText(sprBatch, "DefaultFont", x + 4 - (int)offset, y, 0.5f, _placeholder, placeholderColor);
        else
            TextRenderer.DrawText(sprBatch, "DefaultFont", x + 4 - (int)offset, y, 0.5f, text + ((_blinkCursor && Hovered) ? "_" : ""), textColor);
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
        return maxLength == 0 || (text.Length + 1) < maxLength;
    }
}
