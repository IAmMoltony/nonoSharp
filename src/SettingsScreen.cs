using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NonoSharp.UI;

namespace NonoSharp;

public class SettingsScreen
{
    public Button BackButton { get; private set; }

    public SettingsScreen()
    {
        BackButton = new(10, 10, 0, 40, StringManager.GetString("back"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), Keys.Escape, true);
    }

    public void Draw(SpriteBatch sprBatch)
    {
        GraphicsDevice graphDev = sprBatch.GraphicsDevice;
        Rectangle headingRect = new(0, 15, graphDev.Viewport.Bounds.Width, 100);
        TextRenderer.DrawTextCenter(sprBatch, "DefaultFont", 0.9f, StringManager.GetString("settingsButton"), Color.White, headingRect);

        BackButton.Draw(sprBatch);
    }

    public void Update(MouseState mouse, MouseState mouseOld, KeyboardState kb, KeyboardState kbOld)
    {
        BackButton.Update(mouse, mouseOld, kb, kbOld);
    }
}
