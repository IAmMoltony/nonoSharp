using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NonoSharp.UI;

namespace NonoSharp.Editor;

public class SetSizeState
{
    private NumberTextBox _sizeBox;

    public Button OKButton { get; private set; }
    public Button BackButton { get; private set; }

    public SetSizeState()
    {
        _sizeBox = new(0, 0, 50, Color.DarkGray, Color.Gray, Color.White, Color.White, 25);
        OKButton = new(0, 0, 45, 40, "OK", Color.DarkGreen, Color.Green);
        BackButton = new(10, 10, 70, 40, "Back", Color.DarkGreen, Color.Green);
    }

    public void Draw(SpriteBatch sprBatch)
    {
        drawSizeBox(sprBatch);
        TextRenderer.DrawTextCenter(sprBatch, "notosans", 0, 0, 0.6f, "Enter board size:", Color.White, new(0, _sizeBox.y - 26,
            sprBatch.GraphicsDevice.Viewport.Bounds.Width, 2));
        drawOKButton(sprBatch);
        BackButton.Draw(sprBatch);
    }

    public void Update(MouseState mouse, MouseState mouseOld, KeyboardState kb, KeyboardState kbOld)
    {
        _sizeBox.Update(mouse, mouseOld, kb, kbOld);
        OKButton.Update(mouse, mouseOld, kb, kbOld);
        BackButton.Update(mouse, mouseOld, kb, kbOld);
    }

    public void UpdateInput(object sender, TextInputEventArgs tiea)
    {
        _sizeBox.UpdateInput(sender, tiea);
    }

    public int GetSize()
    {
        return _sizeBox.GetNumberValue();
    }

    private void drawSizeBox(SpriteBatch sprBatch)
    {
        _sizeBox.x = sprBatch.GraphicsDevice.Viewport.Bounds.Width / 2 - _sizeBox.Width / 2;
        _sizeBox.y = sprBatch.GraphicsDevice.Viewport.Bounds.Height / 2 - TextBox.Height / 2;
        _sizeBox.Draw(sprBatch);
    }

    private void drawOKButton(SpriteBatch sprBatch)
    {
        OKButton.x = sprBatch.GraphicsDevice.Viewport.Bounds.Width / 2 - OKButton.width / 2;
        OKButton.y = _sizeBox.y + TextBox.Height + 10;
        OKButton.Draw(sprBatch);
    }
}