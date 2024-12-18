using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NonoSharp.UI;

namespace NonoSharp.Editor;

public class AutoSaveNotice
{
    public static readonly int ButtonSpacing = 10;

    private Button _continueButton;
    private Button _restartButton;
    private Button _cancelButton;

    public AutoSaveNotice()
    {
        _continueButton = new(0, 0, 0, 45, StringManager.GetString("continue"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), true);
        _restartButton = new(0, 0, 0, 45, StringManager.GetString("restart"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), true);
        _cancelButton = new(0, 0, 0, 45, StringManager.GetString("cancel"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), true);
    }

    public void Draw(SpriteBatch sprBatch)
    {
        TextRenderer.DrawTextCenter(sprBatch, "DefaultFont", 0.6f, StringManager.GetString("autoSaveNotice"), Color.White, new(0, sprBatch.GraphicsDevice.Viewport.Bounds.Height / 2 - 41, sprBatch.GraphicsDevice.Viewport.Bounds.Width, 2));
        _continueButton.Draw(sprBatch);
        _restartButton.Draw(sprBatch);
        _cancelButton.Draw(sprBatch);
    }

    public void Update(GraphicsDevice graphDev, MouseState mouse, MouseState mouseOld, KeyboardState kb, KeyboardState kbOld)
    {
        _continueButton.Update(mouse, mouseOld, kb, kbOld);
        _restartButton.Update(mouse, mouseOld, kb, kbOld);
        _cancelButton.Update(mouse, mouseOld, kb, kbOld);

        int totalWidth = _continueButton.width + _restartButton.width + _cancelButton.width + ButtonSpacing * 20;

        _continueButton.x = graphDev.Viewport.Bounds.Width / 2 - totalWidth / 2;
        _restartButton.x = _continueButton.x + _continueButton.width + ButtonSpacing;
        _cancelButton.x = _restartButton.x + _restartButton.width + ButtonSpacing;

        int buttonY = graphDev.Viewport.Bounds.Height / 2;
        _continueButton.y = buttonY;
        _restartButton.y = buttonY;
        _cancelButton.y = buttonY;
    }
}