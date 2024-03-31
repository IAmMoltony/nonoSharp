using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using NonoSharp.UI;

namespace NonoSharp;

public class MainMenu
{
    public Button PlayButton { get; private set; }
    public Button QuitButton { get; private set; }
    public Button EditorButton { get; private set; }

    public MainMenu()
    {
        PlayButton = new(0, 0, 0, 60, StringManager.GetString("playButton"), Color.DarkGreen, Color.Green, true);
        EditorButton = new(0, 0, 120, 60, StringManager.GetString("editorButton"), Color.DarkGreen, Color.Green);
        QuitButton = new(0, 0, 120, 60, StringManager.GetString("quitButton"), Color.DarkGreen, Color.Green);
    }

    public void Update(MouseState mouse, MouseState mouseOld, KeyboardState kb, KeyboardState kbOld, GraphicsDevice graphDev)
    {
        PlayButton.x = graphDev.Viewport.Bounds.Width / 2 - PlayButton.width / 2;
        PlayButton.y = graphDev.Viewport.Bounds.Height / 2 - PlayButton.height / 2;
        PlayButton.Update(mouse, mouseOld, kb, kbOld);

        EditorButton.x = PlayButton.x;
        EditorButton.y = PlayButton.y + 70;
        EditorButton.Update(mouse, mouseOld, kb, kbOld);

        QuitButton.x = PlayButton.x;
        QuitButton.y = PlayButton.y + 70 * 2;
        QuitButton.Update(mouse, mouseOld, kb, kbOld);
    }

    public void Draw(SpriteBatch sprBatch)
    {
        GraphicsDevice graphDev = sprBatch.GraphicsDevice;

        Rectangle nameRect = new(0, PlayButton.y - 100, graphDev.Viewport.Bounds.Width, 100);
        TextRenderer.DrawTextCenter(sprBatch, "notosans", 0, 0, 1.0f, "nonoSharp", Color.White, nameRect);

        PlayButton.Draw(sprBatch);
        EditorButton.Draw(sprBatch);
        QuitButton.Draw(sprBatch);
    }
}
