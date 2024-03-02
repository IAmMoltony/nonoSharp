using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace NonoSharp.Editor;

public enum EditorState
{
    SetSize
}

public class Editor
{
    private EditorState _state;
    private SetSizeState _setSize;

    public Editor()
    {
        _state = EditorState.SetSize;
        _setSize = new();
    }

    public void Update(MouseState mouse, MouseState mouseOld, KeyboardState kb, KeyboardState kbOld)
    {
        switch (_state)
        {
            case EditorState.SetSize:
                _setSize.Update(mouse, mouseOld, kb, kbOld);
                break;
        }
    }

    public void Draw(SpriteBatch sprBatch)
    {
        switch (_state)
        {
            case EditorState.SetSize:
                _setSize.Draw(sprBatch);
                break;
        }
    }

    public void UpdateInput(object sender, TextInputEventArgs tiea)
    {
        if (_state == EditorState.SetSize)
            _setSize.UpdateInput(sender, tiea);
    }
}