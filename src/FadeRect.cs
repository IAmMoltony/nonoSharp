using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NonoSharp;

public enum FadeRectMode
{
    FadeIn,
    FadeOut
}

public class FadeRect
{
    public Rectangle rect;
    public Color color1, color2;
    public FadeRectMode mode;
    private Color _color;

    public FadeRect(Rectangle rect, Color color1, Color color2)
    {
        mode = FadeRectMode.FadeOut;
        _color = color1;
        this.color1 = color1;
        this.color2 = color2;
        this.rect = rect;
    }

    public void Update()
    {
        switch (mode)
        {
            case FadeRectMode.FadeOut:
                doFade(color1);
                break;
            case FadeRectMode.FadeIn:
                doFade(color2);
                break;
        }
    }

    public void Draw(SpriteBatch batch)
    {
        RectRenderer.DrawRect(rect, _color, batch);
    }

    private void doFade(Color color)
    {
        _color.R = (byte)MathHelper.Lerp(_color.R, color.R, 0.07f);
        _color.G = (byte)MathHelper.Lerp(_color.G, color.G, 0.07f);
        _color.B = (byte)MathHelper.Lerp(_color.B, color.B, 0.07f);
    }
}
