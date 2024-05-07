using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NonoSharp.UI;
using System.Linq;

namespace NonoSharp;

public class MainMenu
{
    public Button PlayButton { get; private set; }
    public Button QuitButton { get; private set; }
    public Button EditorButton { get; private set; }
    public Button SettingsButton { get; private set; }

    public MainMenu()
    {
        PlayButton = new(0, 0, 0, 60, StringManager.GetString("playButton"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), Keys.P, true);
        EditorButton = new(0, 0, 120, 60, StringManager.GetString("editorButton"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), Keys.E, true);
        SettingsButton = new(0, 0, 120, 60, StringManager.GetString("settingsButton"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), Keys.S, true);
        QuitButton = new(0, 0, 120, 60, StringManager.GetString("quitButton"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), new[] { Keys.Q, Keys.Escape }, true);
    }

    public void Update(MouseState mouse, MouseState mouseOld, KeyboardState kb, KeyboardState kbOld, GraphicsDevice graphDev)
    {
        PlayButton.x = (graphDev.Viewport.Bounds.Width / 2) - (PlayButton.width / 2);
        PlayButton.y = (graphDev.Viewport.Bounds.Height / 2) - (PlayButton.height / 2);
        PlayButton.Update(mouse, mouseOld, kb, kbOld);

        EditorButton.x = PlayButton.x;
        EditorButton.y = PlayButton.y + 70;
        EditorButton.Update(mouse, mouseOld, kb, kbOld);

        SettingsButton.x = PlayButton.x;
        SettingsButton.y = PlayButton.y + 140;
        SettingsButton.Update(mouse, mouseOld, kb, kbOld);

        QuitButton.x = PlayButton.x;
        QuitButton.y = PlayButton.y + 210;
        QuitButton.Update(mouse, mouseOld, kb, kbOld);

        updateButtonWidths();
    }

    public void Draw(SpriteBatch sprBatch)
    {
        GraphicsDevice graphDev = sprBatch.GraphicsDevice;

        Rectangle nameRect = new(0, PlayButton.y - 100, graphDev.Viewport.Bounds.Width, 100);
        TextRenderer.DrawTextCenter(sprBatch, "DefaultFont", 1.0f, "nonoSharp", Color.White, nameRect);

        PlayButton.Draw(sprBatch);
        EditorButton.Draw(sprBatch);
        SettingsButton.Draw(sprBatch);
        QuitButton.Draw(sprBatch);
    }

    private void updateButtonWidths()
    {
        // find the biggest button width
        int maxWidth = new[] { PlayButton.width, EditorButton.width, SettingsButton.width, QuitButton.width }.Max();

        // disable dynamic width
        PlayButton.isDynamicWidth = false;
        EditorButton.isDynamicWidth = false;
        SettingsButton.isDynamicWidth = false;
        QuitButton.isDynamicWidth = false;

        // set the width of all buttons to the biggest width
        PlayButton.width = maxWidth;
        EditorButton.width = maxWidth;
        SettingsButton.width = maxWidth;
        QuitButton.width = maxWidth;

        // enable dynamic width back
        PlayButton.isDynamicWidth = true;
        EditorButton.isDynamicWidth = true;
        SettingsButton.isDynamicWidth = true;
        QuitButton.isDynamicWidth = true;
    }
}
