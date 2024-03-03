using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace NonoSharp.Editor;

public class SaveLevelState
{
    private UI.TextBox _levelNameBox;
    private bool _isWindows;

    public SaveLevelState()
    {
        _levelNameBox = new(0, 0, 200, Color.DarkGray, Color.Gray, Color.White, Color.White, 230);
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
    }

    public void Update(MouseState mouse, MouseState mouseOld, KeyboardState kb, KeyboardState kbOld)
    {
        _levelNameBox.Update(mouse, mouseOld, kb, kbOld);
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
}