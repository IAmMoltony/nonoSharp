using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NonoSharp.UI;

namespace NonoSharp.Editor;

public class AutoSaveNotice
{
    public static readonly int ButtonSpacing = 10;

    public Button ContinueButton { get; private set; }
    public Button RestartButton { get; private set; }
    public Button CancelButton { get; private set; }

    public AutoSaveNotice()
    {
        ContinueButton = new(0, 0, 0, 45, StringManager.GetString("continue"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), true);
        RestartButton = new(0, 0, 0, 45, StringManager.GetString("restart"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), true);
        CancelButton = new(0, 0, 0, 45, StringManager.GetString("cancel"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), true);
    }

    public void Draw(SpriteBatch sprBatch)
    {
        TextRenderer.DrawTextCenter(sprBatch, "DefaultFont", 0.6f, StringManager.GetString("autoSaveNotice"), Color.White, new(0, sprBatch.GraphicsDevice.Viewport.Bounds.Height / 2 - 41, sprBatch.GraphicsDevice.Viewport.Bounds.Width, 2));
        ContinueButton.Draw(sprBatch);
        RestartButton.Draw(sprBatch);
        CancelButton.Draw(sprBatch);
    }

    public void Update(GraphicsDevice graphDev, MouseState mouse, MouseState mouseOld, KeyboardState kb, KeyboardState kbOld)
    {
        ContinueButton.Update(mouse, mouseOld, kb, kbOld);
        RestartButton.Update(mouse, mouseOld, kb, kbOld);
        CancelButton.Update(mouse, mouseOld, kb, kbOld);

        int totalWidth = ContinueButton.width + RestartButton.width + CancelButton.width + ButtonSpacing * 2;

        ContinueButton.x = graphDev.Viewport.Bounds.Width / 2 - totalWidth / 2;
        RestartButton.x = ContinueButton.x + ContinueButton.width + ButtonSpacing;
        CancelButton.x = RestartButton.x + RestartButton.width + ButtonSpacing;

        int buttonY = graphDev.Viewport.Bounds.Height / 2;
        ContinueButton.y = buttonY;
        RestartButton.y = buttonY;
        CancelButton.y = buttonY;
    }
}