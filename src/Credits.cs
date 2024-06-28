using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NonoSharp.UI;

namespace NonoSharp;

public class Credits : IGameState
{
    public Button BackButton { get; private set; }

    public Credits()
    {
        BackButton = new(10, 10, 0, 40, StringManager.GetString("back"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), true);
    }

    public IGameState? Update(MouseState mouse, MouseState mouseOld, KeyboardState kb, KeyboardState kbOld, GraphicsDevice graphDev, out GameState? newState, ref LevelMetadata levelMetadata, bool hasFocus)
    {
        newState = null;
        BackButton.Update(mouse, mouseOld, kb, kbOld);
        if (BackButton.IsClicked)
        {
            return new SettingsScreen();
        }

        return null;
    }

    public void Draw(SpriteBatch sprBatch)
    {
        Rectangle headingRect = new(0, 15, sprBatch.GraphicsDevice.Viewport.Bounds.Width, 100);
        TextRenderer.DrawTextCenter(sprBatch, "DefaultFont", 0.9f, StringManager.GetString("credits"), Color.White, headingRect);

        // TODO put into file

        BackButton.Draw(sprBatch);
        TextRenderer.DrawText(sprBatch, "DefaultFont", 10, 120, 0.4f, $"nonoSharp {GameVersion.GetGameVersion()}", Color.White);
        TextRenderer.DrawText(sprBatch, "DefaultFont", 10, 140, 0.4f, "Created by Moltony, 2024", Color.White);
        TextRenderer.DrawText(sprBatch, "DefaultFont", 10, 160, 0.4f, "Licensed under the MIT license", Color.White);
        TextRenderer.DrawText(sprBatch, "DefaultFont", 10, 180, 0.4f, "Powered by MonoGame", Color.White);
        TextRenderer.DrawText(sprBatch, "DefaultFont", 10, 230, 0.4f, "Tile place sound: Pop_5.aif by SunnySideSound -- https://freesound.org/s/67091/ -- License: Attribution 4.0", Color.White);
        TextRenderer.DrawText(sprBatch, "DefaultFont", 10, 250, 0.4f, "Cross place sound: pencil.wav by zakkolar -- https://freesound.org/s/431438/ -- License: Creative Commons 0", Color.White);
        TextRenderer.DrawText(sprBatch, "DefaultFont", 10, 270, 0.4f, "Font: Cascadia Mono, licensed under Open Font License", Color.White);
    }
}
