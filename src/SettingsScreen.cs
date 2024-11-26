using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NonoSharp.UI;

namespace NonoSharp;

public class SettingsScreen : IGameState
{
    public const int DialogWidth = 600;
    public const int DialogHeight = 280;

    public Button BackButton { get; private set; }
    public Button CreditsButton { get; private set; }
    public Button KeySettingsButton { get; private set; }
    public Button AccentColorButton { get; private set; }

    private readonly CheckBox _enableHintsBox;
    private readonly CheckBox _showBgBox;
    private readonly CheckBox _enableSoundBox;

    // TODO reusable dialog code
    private Rectangle _dialogRect;
    private bool _setAccentColor;

    public SettingsScreen()
    {
        BackButton = new(10, 10, 0, 40, StringManager.GetString("back"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), Keys.Escape, true);
        CreditsButton = new(10, 0, 0, 40, StringManager.GetString("credits"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), Keys.C, true);
        KeySettingsButton = new(10, 0, 0, 40, StringManager.GetString("keySettings"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), Keys.K, true);

        // Settings controls:
        _enableHintsBox = new(40, 110, StringManager.GetString("enableHints"), Color.Gray, Color.DarkGray, Settings.GetBool("enableHints"));
        _showBgBox = new(40, 140, StringManager.GetString("showBgGrid"), Color.Gray, Color.DarkGray, Settings.GetBool("showBgGrid"));
        _enableSoundBox = new(40, 170, StringManager.GetString("enableSound"), Color.Gray, Color.DarkGray, Settings.GetBool("sound"));
        AccentColorButton = new(40, 215, 0, 40, StringManager.GetString("changeAccentColor"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), true);

        _dialogRect = new();
        _setAccentColor = false;
    }

    public void Draw(SpriteBatch sprBatch)
    {
        GraphicsDevice graphDev = sprBatch.GraphicsDevice;
        Rectangle headingRect = new(0, 15, graphDev.Viewport.Bounds.Width, 100);
        TextRenderer.DrawTextCenter(sprBatch, "DefaultFont", 0.9f, StringManager.GetString("settingsButton"), Color.White, headingRect);

        BackButton.Draw(sprBatch);
        CreditsButton.Draw(sprBatch);
        KeySettingsButton.Draw(sprBatch);

        _enableHintsBox.Draw(sprBatch);
        _showBgBox.Draw(sprBatch);
        _enableSoundBox.Draw(sprBatch);
        AccentColorButton.Draw(sprBatch);

        if (_setAccentColor)
        {
            Rectangle dialogTextRect = new();
            
            RectRenderer.DrawRect(new(0, 0, graphDev.Viewport.Bounds.Width, graphDev.Viewport.Bounds.Height), new(Color.Black, 0.5f), sprBatch);
            dialogTextRect = _dialogRect;
            dialogTextRect.Y -= 100;
            RectRenderer.DrawRect(_dialogRect, Settings.GetDarkAccentColor(), sprBatch);
            RectRenderer.DrawRectOutline(_dialogRect, Settings.GetAccentColor(), 2, sprBatch);

            TextRenderer.DrawTextCenter(sprBatch, "DefaultFont", 0.8f, StringManager.GetString("changeAccentColor"), Color.White, dialogTextRect);
        }
    }

    public IGameState? Update(MouseState mouse, MouseState mouseOld, KeyboardState kb, KeyboardState kbOld, GraphicsDevice graphDev, ref LevelMetadata levelMetadata, bool hasFocus)
    {
        if (_setAccentColor)
        {
            _dialogRect.X = (graphDev.Viewport.Bounds.Width / 2) - (_dialogRect.Width / 2);
            _dialogRect.Y = (graphDev.Viewport.Bounds.Height / 2) - (DialogHeight / 2);
            _dialogRect.Width = DialogWidth;
            _dialogRect.Height = DialogHeight;
        }
        else
        {
            _enableHintsBox.Update(mouse, mouseOld, kb, kbOld);
            _showBgBox.Update(mouse, mouseOld, kb, kbOld);
_enableSoundBox.Update(mouse, mouseOld, kb, kbOld);
            AccentColorButton.Update(mouse, mouseOld, kb, kbOld);

            if (AccentColorButton.IsClicked)
            {
                _setAccentColor = true;
            }

            BackButton.Update(mouse, mouseOld, kb, kbOld);
            if (BackButton.IsClicked)
            {
                saveSettings();
                return new MainMenu();
            }

            CreditsButton.y = graphDev.Viewport.Bounds.Height - 50;
            CreditsButton.Update(mouse, mouseOld, kb, kbOld);
            if (CreditsButton.IsClicked)
                return new Credits();

            KeySettingsButton.y = CreditsButton.y;
            KeySettingsButton.x = CreditsButton.x + CreditsButton.width + 10;
            KeySettingsButton.Update(mouse, mouseOld, kb, kbOld);
            if (KeySettingsButton.IsClicked)
                return new KeySettingsScreen();
        }

        return null;
    }

    public void saveSettings()
    {
        Settings.Set("enableHints", _enableHintsBox.isChecked);
        Settings.Set("showBgGrid", _showBgBox.isChecked);
        Settings.Set("sound", _enableSoundBox.isChecked);
        Settings.Save();
    }
}
