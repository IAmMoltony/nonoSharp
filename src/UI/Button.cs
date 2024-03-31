using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace NonoSharp.UI;

public class Button : UIElement
{
    public int width, height;
    public string text;
    public Color fillColor, outlineColor;

    public bool IsHovered { get; private set; }
    public bool IsClicked { get; private set; }

    private FadeRect _fr;
    private bool _isDynamicWidth;
    private int _dynamicWidthPad;

    public Button(int x, int y, int width, int height, string text, Color fillColor, Color outlineColor, bool dynamicWidth = false, int dynamicWidthPad = 10) : base(x, y)
    {
        this.width = width;
        this.height = height;
        this.text = text;
        this.fillColor = fillColor;
        this.outlineColor = outlineColor;

        IsHovered = false;
        IsClicked = false;
        _isDynamicWidth = dynamicWidth;
        _dynamicWidthPad = dynamicWidthPad;

        createFadeRect();

        if (this.width < 0 || this.height < 0)
            throw new ArgumentException("Button width and height must not be negative");
    }

    public override void Draw(SpriteBatch sprBatch)
    {
        _fr.rect = getRect();
        _fr.Draw(sprBatch);
        RectRenderer.DrawRectOutline(getRect(), IsHovered ? fillColor : outlineColor, 2, sprBatch);
        TextRenderer.DrawTextCenter(sprBatch, "DefaultFont", x, y, 0.5f, text, Color.White, getRect());
    }

    public override void Update(MouseState mouse, MouseState mouseOld, KeyboardState keyboard, KeyboardState keyboardOld)
    {
        if (_isDynamicWidth)
            updateDynamicWidth();

        Rectangle rect = getRect();
        IsHovered = mouse.X >= rect.X && mouse.Y >= rect.Y && mouse.X <= rect.X + rect.Width && mouse.Y <= rect.Y + rect.Height;
        IsClicked = IsHovered && mouse.LeftButton == ButtonState.Pressed && mouseOld.LeftButton == ButtonState.Released;
        if (IsHovered)
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
        width = _dynamicWidthPad + (int)(TextRenderer.MeasureString("DefaultFont", text).X * 0.5f); // TODO extract font name + scale into members
    }

    private Rectangle getRect() => new(x, y, width, height);
}
