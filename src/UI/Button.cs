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

    public Button(int x, int y, int width, int height, string text, Color fillColor, Color outlineColor) : base(x, y)
    {
        this.width = width;
        this.height = height;
        this.text = text;
        this.fillColor = fillColor;
        this.outlineColor = outlineColor;

        IsHovered = false;
        IsClicked = false;

        if (this.width < 0 || this.height < 0)
            throw new ArgumentException("Button width and height must not be negative");
    }

    public override void Draw(SpriteBatch sprBatch)
    {
        RectRenderer.DrawRect(getRect(), IsHovered ? outlineColor : fillColor, sprBatch);
        RectRenderer.DrawRectOutline(getRect(), IsHovered ? fillColor : outlineColor, 2, sprBatch);
        TextRenderer.DrawTextCenter(sprBatch, "notosans", x, y, 0.5f, text, Color.White, getRect());
    }

    public override void Update(MouseState mouse, MouseState mouseOld, KeyboardState keyboard, KeyboardState keyboardOld)
    {
        Rectangle rect = getRect();
        IsHovered = (mouse.X >= rect.X && mouse.Y >= rect.Y && mouse.X <= rect.X + rect.Width && mouse.Y <= rect.Y + rect.Height);
        IsClicked = IsHovered && (mouse.LeftButton == ButtonState.Pressed && mouseOld.LeftButton == ButtonState.Released);
    }

    private Rectangle getRect()
    {
        return new Rectangle(x, y, width, height);
    }
}
