using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace NonoSharp;

public class TextRenderer
{
    private static Dictionary<string, SpriteFont> _fonts;

    private TextRenderer()
    {
    }

    public static void LoadFont(string fontName, string fontAsset, ContentManager content)
    {
        if (_fonts == null)
            _fonts = new();
        _fonts[fontName] = content.Load<SpriteFont>(fontAsset);
    }

    public static void DrawText(SpriteBatch batch, string font, int x, int y, string text, Color color)
    {
        batch.DrawString(_fonts[font], text, new Vector2((float)x, (float)y), color);
    }

    public static void DrawText(SpriteBatch batch, string font, int x, int y, float scale, string text, Color color)
    {
        batch.DrawString(_fonts[font], text, new Vector2((float)x, (float)y), color, 0, Vector2.Zero, scale, SpriteEffects.None, 1.0f);
    }

    public static void DrawTextCenter(SpriteBatch batch, string font, int x, int y, float scale, string text, Color color, Rectangle rect)
    {
        Vector2 textSize = _fonts[font].MeasureString(text);
        float centerX = rect.X + (rect.Width - textSize.X * scale) / 2;
        float centerY = rect.Y + (rect.Height - textSize.Y * scale) / 2;
        batch.DrawString(_fonts[font], text, new Vector2(centerX, centerY), color, 0, Vector2.Zero, scale, SpriteEffects.None, 1.0f);
    }
}
