using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NonoSharp;

public enum FadeRectMode
{
    FadeIn,
    FadeOut
}

// TODO use Color.Lerp in this class.

public class FadeRect
{
    public Rectangle rect;
    public Color color1, color2;
    public FadeRectMode mode;
    public float r;
    public float g;
    public float b;
    public float lerpTime;

    public FadeRect(Rectangle rect, Color color1, Color color2, float lerpTime = 0.07f)
    {
        mode = FadeRectMode.FadeOut;
        r = color1.R;
        g = color1.G;
        b = color1.B;
        this.lerpTime = lerpTime;
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
        Color color = new((int)r, (int)g, (int)b);
        RectRenderer.DrawRect(rect, color, batch);
    }

    private void doFade(Color color)
    {
        r = MathHelper.Lerp(r, color.R, lerpTime);
        g = MathHelper.Lerp(g, color.G, lerpTime);
        b = MathHelper.Lerp(b, color.B, lerpTime);
    }
}
