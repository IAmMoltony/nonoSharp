using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Serilog;

namespace NonoSharp.UI;

public class CheckBox : UIElement
{
    public static readonly int Size = 26;
    public static readonly float LerpTime = 0.16f;

    public static Texture2D? TextureCheck { get; private set; }

    public bool isChecked;

    private Color _fillColor;
    private Color _outlineColor;
    private bool _isHovered;
    private readonly string _text;
    private FadeRect _fr;
    private FadeRect _frOutline;

    public static void LoadTextures(ContentManager content)
    {
        Log.Logger.Information("Loading checkbox textures");
        TextureCheck = content.Load<Texture2D>("image/check");
    }

    public CheckBox(int x, int y, string text, Color fillColor, Color outlineColor, bool isChecked = false) : base(x, y)
    {
        this.isChecked = isChecked;
        _fillColor = fillColor;
        _outlineColor = outlineColor;
        _isHovered = false;
        _text = text;
        _fr = new(getRect(), fillColor, outlineColor, LerpTime);
        _frOutline = new(getRect(), outlineColor, fillColor, LerpTime, true, 2);
    }

    public override void Draw(SpriteBatch sprBatch)
    {
        Rectangle rect = getRect();
        Color fc = _isHovered ? _outlineColor : _fillColor;
        Color oc = _isHovered ? _fillColor : _outlineColor;
        _fr.Draw(sprBatch);
        _frOutline.Draw(sprBatch);

        Color checkColor = fc.IsLight() ? Color.Black : Color.White;

        if (isChecked && TextureCheck != null)
            sprBatch.Draw(TextureCheck, new Vector2(x, y), checkColor);

        drawText(sprBatch, rect);
    }

    public override void Update(MouseState mouse, MouseState mouseOld, KeyboardState keyboard, KeyboardState keyboardOld)
    {
        _isHovered = mouse.X > x && mouse.Y > y && mouse.X < x + Size && mouse.Y < y + Size;

        if (_isHovered && mouse.LeftButton == ButtonState.Pressed && mouseOld.LeftButton == ButtonState.Released)
            isChecked = !isChecked;

        if (_isHovered)
        {
            _fr.mode = FadeRectMode.FadeIn;
            _frOutline.mode = FadeRectMode.FadeIn;
        }
        else
        {
            _fr.mode = FadeRectMode.FadeOut;
            _frOutline.mode = FadeRectMode.FadeOut;
        }
        _fr.Update();
        _frOutline.Update();
    }

    private Rectangle getRect()
    {
        return new(x, y, Size, Size);
    }

    private void drawText(SpriteBatch sprBatch, Rectangle rect)
    {
        TextRenderer.DrawText(sprBatch, "DefaultFont", x + rect.Width + 10, y, 0.4f, _text, Color.White);
    }
}
