using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace NonoSharp.Editor;

public enum EditorState
{
    SetSize,
    Editor
}

public class Editor
{
    private EditorState _state;
    private SetSizeState _setSize;

    private EditorBoard _board;

    public Editor()
    {
        _state = EditorState.SetSize;
        _setSize = new();
        _board = new();
    }

    public void Update(MouseState mouse, MouseState mouseOld, KeyboardState kb, KeyboardState kbOld, GraphicsDevice graphDev)
    {
        switch (_state)
        {
            case EditorState.SetSize:
                _setSize.Update(mouse, mouseOld, kb, kbOld);
                if (_setSize.OKButton.IsClicked && _setSize.GetSize() > 0)
                {
                    _state = EditorState.Editor;
                    _board.Make(_setSize.GetSize());
                }
                break;
            case EditorState.Editor:
                _board.Update(mouse, mouseOld, graphDev);
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
            case EditorState.Editor:
                _board.Draw(sprBatch);
                break;
        }
    }

    public void UpdateInput(object sender, TextInputEventArgs tiea)
    {
        if (_state == EditorState.SetSize)
            _setSize.UpdateInput(sender, tiea);
    }
}