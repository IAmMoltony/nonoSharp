using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NonoSharp.UI;

namespace NonoSharp;

public class Credits
{
    public Button BackButton { get; private set; }

    public Credits()
    {
        BackButton = new(10, 10, 0, 40, StringManager.GetString("back"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), true);
    }

    public void Update(MouseState mouse, MouseState mouseOld, KeyboardState kb, KeyboardState kbOld)
    {
        BackButton.Update(mouse, mouseOld, kb, kbOld);
    }

    public void Draw(SpriteBatch sprBatch)
    {
        Rectangle headingRect = new(0, 15, sprBatch.GraphicsDevice.Viewport.Bounds.Width, 100);
        TextRenderer.DrawTextCenter(sprBatch, "DefaultFont", 0.9f, StringManager.GetString("credits"), Color.White, headingRect);

        BackButton.Draw(sprBatch);
        TextRenderer.DrawText(sprBatch, "DefaultFont", 10, 120, 0.4f, $"nonoSharp {GameVersion.GetGameVersion()}", Color.White);
        TextRenderer.DrawText(sprBatch, "DefaultFont", 10, 140, 0.4f, "Created by Moltony, 2024", Color.White);
        TextRenderer.DrawText(sprBatch, "DefaultFont", 10, 160, 0.4f, "Licensed under the MIT license", Color.White);
        TextRenderer.DrawText(sprBatch, "DefaultFont", 10, 180, 0.4f, "Powered by MonoGame", Color.White);
        TextRenderer.DrawText(sprBatch, "DefaultFont", 10, 230, 0.4f, "Tile place sound: Pop_5 from Pops pack by SunnySideSound", Color.White);
    }
}
