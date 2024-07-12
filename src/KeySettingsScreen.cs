using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using NonoSharp.UI;
using Serilog;

namespace NonoSharp;

public class KeySettingsScreen : IGameState
{
    public Button BackButton { get; private set; }

    public KeySettingsScreen()
    {
        BackButton = new(10, 10, 0, 40, StringManager.GetString("back"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), Keys.Escape, true);
    }

    public void Draw(SpriteBatch sprBatch)
    {
        Rectangle headingRect = new(0, 15, sprBatch.GraphicsDevice.Viewport.Bounds.Width, 100);
        TextRenderer.DrawTextCenter(sprBatch, "DefaultFont", 0.9f, StringManager.GetString("keySettings"), Color.White, headingRect);

        BackButton.Draw(sprBatch);
    }

    public IGameState? Update(MouseState mouse, MouseState mouseOld, KeyboardState kb, KeyboardState kbOld, GraphicsDevice graphDev, ref LevelMetadata levelMetadata, bool hasFocus)
    {
        BackButton.Update(mouse, mouseOld, kb, kbOld);
        if (BackButton.IsClicked)
            return new SettingsScreen();

        if (kb.AnyKeyPressed() && !kbOld.AnyKeyPressed())
            Log.Logger.Information($"pressed {kb.GetPressedKeys()[0]} code {(int)kb.GetPressedKeys()[0]}");

        return null;
    }
}
