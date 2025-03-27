using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System.Linq;

namespace NonoSharp.Editor;

public class SaveLevelState
{
    private readonly UI.TextBox _levelNameBox;
    private readonly UI.Button _saveButton;
    private bool _saved;

    public UI.Button OKButton { get; private set; }
    public UI.Button BackButton { get; private set; }

    public SaveLevelState()
    {
        _levelNameBox = new(0, 0, 200, Color.DarkGray, Color.Gray, Color.White, Color.White, 230);
        _saveButton = new(0, 0, 0, 40, StringManager.GetString("save"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), Keys.Enter, true);
        _saved = false;

        OKButton = new(0, 0, 0, 40, StringManager.GetString("ok"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), Keys.Enter, true);
        BackButton = new(10, 10, 0, 40, StringManager.GetString("back"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), Keys.Escape, true);

        // Some operating systems (especially windows) don't allow certain characters in file names. I don't
        // really want to implement an error checker for the save button so I'm just going to make
        // every invalid file name (and path name for good measure) character illegal.
        char[] invalidPathChars = Path.GetInvalidPathChars();
        char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
        _levelNameBox.illegalChars = invalidPathChars.Union(invalidFileNameChars).ToList();

        // make it so that level names can't have return chars in them
        _levelNameBox.illegalChars.Add('\r');
        _levelNameBox.illegalChars.Add('\n');
    }

    public void Draw(SpriteBatch sprBatch)
    {
        if (!_saved)
        {
            drawNameBox(sprBatch);
            TextRenderer.DrawTextCenter(sprBatch, "DefaultFont", 0.6f, StringManager.GetString("enterLevelName"), Color.White, new(0,
                _levelNameBox.y - 26, sprBatch.GraphicsDevice.Viewport.Bounds.Width, 2));
            drawSaveButton(sprBatch);
            BackButton.Draw(sprBatch);
        }
        else
        {
            TextRenderer.DrawTextCenter(sprBatch, "DefaultFont", 0.6f, StringManager.GetString("levelSaved"), Color.White, new(0,
                _levelNameBox.y - 26, sprBatch.GraphicsDevice.Viewport.Bounds.Width, 2));
            drawOKButton(sprBatch);
        }
    }

    public void Update(MouseState mouse, MouseState mouseOld, KeyboardState kb, KeyboardState kbOld, Board board, int maxHints)
    {
        _levelNameBox.Update(mouse, mouseOld, kb, kbOld);

        if (!_saved)
        {
            _saveButton.disabled = _levelNameBox.text == "";
            _saveButton.Update(mouse, mouseOld, kb, kbOld);

            if (_saveButton.IsClicked)
            {
                BoardSaver.SaveBoard(board, _levelNameBox.text, maxHints);
                File.Delete(Path.Combine(Settings.GetDataFolderPath(), "EditorAutosave.nono"));
                _saved = true;
            }

            BackButton.Update(mouse, mouseOld, kb, kbOld);
        }
        else
        {
            OKButton.Update(mouse, mouseOld, kb, kbOld);
        }
    }

    public void UpdateInput(TextInputEventArgs tiea)
    {
        _levelNameBox.UpdateInput(tiea);
    }

    private void drawNameBox(SpriteBatch sprBatch)
    {
        _levelNameBox.x = (sprBatch.GraphicsDevice.Viewport.Bounds.Width / 2) - (_levelNameBox.width / 2);
        _levelNameBox.y = (sprBatch.GraphicsDevice.Viewport.Bounds.Height / 2) - (UI.TextBox.Height / 2);
        _levelNameBox.Draw(sprBatch);
    }

    private void drawSaveButton(SpriteBatch sprBatch)
    {
        _saveButton.x = (sprBatch.GraphicsDevice.Viewport.Bounds.Width / 2) - (_saveButton.width / 2);
        _saveButton.y = _levelNameBox.y + UI.TextBox.Height + 10;
        _saveButton.Draw(sprBatch);
    }

    private void drawOKButton(SpriteBatch sprBatch)
    {
        OKButton.x = (sprBatch.GraphicsDevice.Viewport.Bounds.Width / 2) - (OKButton.width / 2);
        OKButton.y = _levelNameBox.y + UI.TextBox.Height + 10;
        OKButton.Draw(sprBatch);
    }
}
