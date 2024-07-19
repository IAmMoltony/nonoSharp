using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NonoSharp.UI;
using Serilog;
using System;
using System.IO;

namespace NonoSharp;

public class Credits : IGameState
{
    public Button BackButton { get; private set; }

    private string _creditsText;

    public Credits()
    {
        BackButton = new(10, 10, 0, 40, StringManager.GetString("back"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), true);
        loadCreditsText();
    }

    public IGameState? Update(MouseState mouse, MouseState mouseOld, KeyboardState kb, KeyboardState kbOld, GraphicsDevice graphDev, ref LevelMetadata levelMetadata, bool hasFocus)
    {
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

        BackButton.Draw(sprBatch);
        TextRenderer.DrawTextWrapped(sprBatch, "DefaultFont", 10, 120, 0.4f, string.Format(_creditsText, GameVersion.GetGameVersion()), sprBatch.GraphicsDevice.Viewport.Bounds.Width, Color.White);
    }

    private void loadCreditsText()
    {
        Log.Logger.Information("Loading credits text");
        _creditsText = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "Credits.txt"));
    }
}
