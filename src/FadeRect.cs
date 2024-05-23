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
    public float r;
    public float g;
    public float b;
    public float lerpTime;
    public bool isOutline;
    public int thickness; // used when is outline

    public FadeRect(Rectangle rect, Color color1, Color color2, float lerpTime = 0.07f, bool isOutline = false, int thickness = 1)
    {
        mode = FadeRectMode.FadeOut;
        r = color1.R;
        g = color1.G;
        b = color1.B;
        this.lerpTime = lerpTime;
        this.color1 = color1;
        this.color2 = color2;
        this.rect = rect;
        this.isOutline = isOutline;
        this.thickness = 1;
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
        Color color = new((int)r, (int)g, (int)b);
        if (isOutline)
        {
            RectRenderer.DrawRectOutline(rect, color, thickness, batch);
        }
        else
        {
            RectRenderer.DrawRect(rect, color, batch);
        }
    }

    public void SetColor(Color color)
    {
        r = color.R;
        g = color.G;
        b = color.B;
    }

    private void doFade(Color color)
    {
        r = MathHelper.Lerp(r, color.R, lerpTime);
        g = MathHelper.Lerp(g, color.G, lerpTime);
        b = MathHelper.Lerp(b, color.B, lerpTime);
    }
}
