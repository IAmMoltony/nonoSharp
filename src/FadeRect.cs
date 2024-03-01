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

    private float _r;
    private float _g;
    private float _b;
    private float _lerpTime;

    public FadeRect(Rectangle rect, Color color1, Color color2, float lerpTime = 0.07f)
    {
        mode = FadeRectMode.FadeOut;
        _r = color1.R;
        _g = color1.G;
        _b = color1.B;
        _lerpTime = lerpTime;
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
        Color color = new((int)_r, (int)_g, (int)_b);
        RectRenderer.DrawRect(rect, color, batch);
    }

    private void doFade(Color color)
    {
        _r = MathHelper.Lerp(_r, color.R, _lerpTime);
        _g = MathHelper.Lerp(_g, color.G, _lerpTime);
        _b = MathHelper.Lerp(_b, color.B, _lerpTime);
    }
}
