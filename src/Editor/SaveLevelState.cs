using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace NonoSharp.Editor;

public class SaveLevelState
{
    private UI.TextBox _levelNameBox;
    private UI.Button _saveButton;
    private bool _isWindows;

    public SaveLevelState()
    {
        _levelNameBox = new(0, 0, 200, Color.DarkGray, Color.Gray, Color.White, Color.White, 230);
        _saveButton = new(0, 0, 100, 40, "Save", Color.DarkGreen, Color.Green);
        _isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        if (_isWindows)
        {
            // We're on windows
            // Windows doesn't allow certain characters in file names
            _levelNameBox.illegalChars = new() { '<', '>', ':', '"', '/', '\\', '|', '?', '*' };
        }
        else
        {
            // We're on BSD/MacOS/Linux/something else
            // Generally those OSes only don't allow the forward slash
            _levelNameBox.illegalChars = new() { '/' };
        }
    }

    public void Draw(SpriteBatch sprBatch)
    {
        drawNameBox(sprBatch);
        TextRenderer.DrawTextCenter(sprBatch, "notosans", 0, 0, 0.6f, "Enter level name:", Color.White, new(0,
            _levelNameBox.y - 26, sprBatch.GraphicsDevice.Viewport.Bounds.Width, 2));
        drawSaveButton(sprBatch);
    }

    public void Update(MouseState mouse, MouseState mouseOld, KeyboardState kb, KeyboardState kbOld, Board board)
    {
        _levelNameBox.Update(mouse, mouseOld, kb, kbOld);
        _saveButton.Update(mouse, mouseOld, kb, kbOld);

        if (_saveButton.IsClicked)
        {
            BoardSaver.SaveBoard(board, _levelNameBox.Text);
        }
    }

    public void UpdateInput(object sender, TextInputEventArgs tiea)
    {
        _levelNameBox.UpdateInput(sender, tiea);
    }

    private void drawNameBox(SpriteBatch sprBatch)
    {
        _levelNameBox.x = sprBatch.GraphicsDevice.Viewport.Bounds.Width / 2 - _levelNameBox.Width / 2;
        _levelNameBox.y = sprBatch.GraphicsDevice.Viewport.Bounds.Height / 2 - UI.TextBox.Height / 2;
        _levelNameBox.Draw(sprBatch);
    }

    private void drawSaveButton(SpriteBatch sprBatch)
    {
        _saveButton.x = sprBatch.GraphicsDevice.Viewport.Bounds.Width / 2 - _saveButton.width / 2;
        _saveButton.y = _levelNameBox.y + UI.TextBox.Height + 10;
        _saveButton.Draw(sprBatch);
    }
}
