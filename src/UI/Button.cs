using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace NonoSharp.UI;

public class Button : UIElement
{
    public int width, height;
    public string text;
    public Color fillColor, outlineColor;
    public bool isDynamicWidth;
    public int dynamicWidthPad;
    public bool disabled;
    public Keys[] shortcutKeys;
    public string font;
    public float fontScale;

    public bool IsHovered { get; private set; }
    public bool IsClicked { get; private set; }

    private FadeRect _fr;

    public Button(int x, int y, int width, int height, string text, Color fillColor, Color outlineColor, bool dynamicWidth = false, int dynamicWidthPad = 10) : base(x, y)
    {
        this.width = width;
        this.height = height;
        this.text = text;
        this.fillColor = fillColor;
        this.outlineColor = outlineColor;
        this.dynamicWidthPad = dynamicWidthPad;
        isDynamicWidth = dynamicWidth;
        disabled = false;
        shortcutKeys = new Keys[] { Keys.None };
        font = "DefaultFont";
        fontScale = 0.5f;

        IsHovered = false;
        IsClicked = false;

        createFadeRect();

        if (this.width < 0 || this.height < 0)
            throw new ArgumentException("Button width and height must not be negative");
    }

    public Button(int x, int y, int width, int height, string text, Color fillColor, Color outlineColor, Keys shortcutKey, bool dynamicWidth = false, int dynamicWidthPad = 10) : this(x, y, width, height, text, fillColor, outlineColor, dynamicWidth, dynamicWidthPad)
    {
        this.shortcutKeys = new[] { shortcutKey };
    }

    public Button(int x, int y, int width, int height, string text, Color fillColor, Color outlineColor, Keys[] shortcutKeys, bool dynamicWidth = false, int dynamicWidthPad = 10) : this(x, y, width, height, text, fillColor, outlineColor, dynamicWidth, dynamicWidthPad)
    {
        this.shortcutKeys = shortcutKeys;
    }

    public Button(int x, int y, int width, int height, string text, Color fillColor, Color outlineColor, float fontScale, bool dynamicWidth = false, int dynamicWidthPad = 10) : this(x, y, width, height, text, fillColor, outlineColor, dynamicWidth, dynamicWidthPad)
    {
        this.fontScale = fontScale;
    }

    public Button(int x, int y, int width, int height, string text, Color fillColor, Color outlineColor, float fontScale, string font, bool dynamicWidth = false, int dynamicWidthPad = 10) : this(x, y, width, height, text, fillColor, outlineColor, fontScale, dynamicWidth, dynamicWidthPad)
    {
        this.font = font;
    }

    public Button(int x, int y, int width, int height, string text, Color fillColor, Color outlineColor, float fontScale, Keys shortcutKey, bool dynamicWidth = false, int dynamicWidthPad = 10) : this(x, y, width, height, text, fillColor, outlineColor, dynamicWidth, dynamicWidthPad)
    {
        this.fontScale = fontScale;
        this.shortcutKeys = new Keys[] { shortcutKey };
    }

    public Button(int x, int y, int width, int height, string text, Color fillColor, Color outlineColor, float fontScale, string font, Keys shortcutKey, bool dynamicWidth = false, int dynamicWidthPad = 10) : this(x, y, width, height, text, fillColor, outlineColor, fontScale, dynamicWidth, dynamicWidthPad)
    {
        this.font = font;
        this.shortcutKeys = new Keys[] { shortcutKey };
    }

    public override void Draw(SpriteBatch sprBatch)
    {
        _fr.rect = getRect();
        _fr.Draw(sprBatch);
        RectRenderer.DrawRectOutline(getRect(), (IsHovered && !disabled) ? fillColor : outlineColor, 2, sprBatch);
        TextRenderer.DrawTextCenter(sprBatch, font, fontScale, text, disabled ? Color.Gray : Color.White, getRect());
    }

    public override void Update(MouseState mouse, MouseState mouseOld, KeyboardState keyboard, KeyboardState keyboardOld)
    {
        if (isDynamicWidth)
            updateDynamicWidth();

        Rectangle rect = getRect();
        if (disabled)
        {
            IsClicked = false;
            if (IsHovered)
                // Set the cursor to the "no" shape
                // On my system it shows up as skull and bones
                NonoSharpGame.Cursor = MouseCursor.No;
        }
        else
            IsClicked = (IsHovered && mouse.LeftButton == ButtonState.Pressed && mouseOld.LeftButton == ButtonState.Released) || shortcutKeyPressed(keyboard, keyboardOld);

        IsHovered = mouse.X >= rect.X && mouse.Y >= rect.Y && mouse.X <= rect.X + rect.Width && mouse.Y <= rect.Y + rect.Height;
        if (IsHovered && !disabled)
        {
            _fr.mode = FadeRectMode.FadeIn;
            NonoSharpGame.Cursor = MouseCursor.Hand;
        }
        else
            _fr.mode = FadeRectMode.FadeOut;
        _fr.Update();
    }

    private void createFadeRect()
    {
        _fr = new(new(x, y, width, height), fillColor, outlineColor);
    }

    private void updateDynamicWidth()
    {
        width = dynamicWidthPad + (int)(TextRenderer.MeasureString(font, text).X * fontScale);
    }

    private bool shortcutKeyPressed(KeyboardState kb, KeyboardState kbOld)
    {
        foreach (Keys key in shortcutKeys)
            if (kb.IsKeyDown(key) && !kbOld.IsKeyDown(key))
                return true;
        return false;
    }

    private Rectangle getRect() => new(x, y, width, height);
}
