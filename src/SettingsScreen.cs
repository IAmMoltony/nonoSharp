using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NonoSharp.UI;

namespace NonoSharp;

public class SettingsScreen
{
    public Button BackButton { get; private set; }
    public Button CreditsButton { get; private set; }

    private CheckBox _enableHintsBox;
    private CheckBox _showBgBox;
    private CheckBox _enableSoundBox;

    public SettingsScreen()
    {
        BackButton = new(10, 10, 0, 40, StringManager.GetString("back"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), Keys.Escape, true);
        CreditsButton = new(10, 0, 0, 40, StringManager.GetString("credits"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), Keys.C, true);

        _enableHintsBox = new(40, 110, StringManager.GetString("enableHints"), Color.Gray, Color.DarkGray, Settings.GetBool("enableHints"));
        _showBgBox = new(40, 140, StringManager.GetString("showBgGrid"), Color.Gray, Color.DarkGray, Settings.GetBool("showBgGrid"));
        _enableSoundBox = new(40, 170, StringManager.GetString("enableSound"), Color.Gray, Color.DarkGray, Settings.GetBool("sound"));
    }

    public void Draw(SpriteBatch sprBatch)
    {
        GraphicsDevice graphDev = sprBatch.GraphicsDevice;
        Rectangle headingRect = new(0, 15, graphDev.Viewport.Bounds.Width, 100);
        TextRenderer.DrawTextCenter(sprBatch, "DefaultFont", 0.9f, StringManager.GetString("settingsButton"), Color.White, headingRect);

        BackButton.Draw(sprBatch);
        CreditsButton.Draw(sprBatch);

        _enableHintsBox.Draw(sprBatch);
        _showBgBox.Draw(sprBatch);
        _enableSoundBox.Draw(sprBatch);
    }

    public void Update(MouseState mouse, MouseState mouseOld, KeyboardState kb, KeyboardState kbOld, GraphicsDevice graphDev)
    {
        _enableHintsBox.Update(mouse, mouseOld, kb, kbOld);
        _showBgBox.Update(mouse, mouseOld, kb, kbOld);
        _enableSoundBox.Update(mouse, mouseOld, kb, kbOld);

        BackButton.Update(mouse, mouseOld, kb, kbOld);
        if (BackButton.IsClicked)
            close();
        CreditsButton.Update(mouse, mouseOld, kb, kbOld);
        CreditsButton.y = graphDev.Viewport.Bounds.Height - 50;
    }

    public void close()
    {
        Settings.Set("enableHints", _enableHintsBox.isChecked);
        Settings.Set("showBgGrid", _showBgBox.isChecked);
        Settings.Set("sound", _enableSoundBox.isChecked);
        Settings.Save();
    }
}
