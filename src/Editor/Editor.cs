using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace NonoSharp.Editor;

public enum EditorState
{
    SetSize,
    Editor,
    SaveLevel,
}

public class Editor
{
    private EditorState _state;
    private SetSizeState _setSize;
    private EditorMain _main;
    private SaveLevelState _saveLevel;

    public Editor()
    {
        _state = EditorState.SetSize;
        _setSize = new();
        _main = new();
        _saveLevel = new();
    }

    public void Update(MouseState mouse, MouseState mouseOld, KeyboardState kb, KeyboardState kbOld, GraphicsDevice graphDev,
                       out GameState newState)
    {
        newState = GameState.None;
        switch (_state)
        {
            case EditorState.SetSize:
                _setSize.Update(mouse, mouseOld, kb, kbOld);
                if (_setSize.OKButton.IsClicked && _setSize.GetSize() > 0)
                {
                    _state = EditorState.Editor;
                    _main.Board.Make(_setSize.GetSize());
                }
                if (_setSize.BackButton.IsClicked)
                {
                    _state = EditorState.SetSize;
                    _setSize = new();
                    newState = GameState.MainMenu;
                }
                break;
            case EditorState.Editor:
                _main.Update(mouse, mouseOld, kb, kbOld, graphDev);
                if (_main.SaveButton.IsClicked)
                    _state = EditorState.SaveLevel;
                break;
            case EditorState.SaveLevel:
                _saveLevel.Update(mouse, mouseOld, kb, kbOld, _main.Board);
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
                _main.Draw(sprBatch);
                break;
            case EditorState.SaveLevel:
                _saveLevel.Draw(sprBatch);
                break;
        }
    }

    public void UpdateInput(object sender, TextInputEventArgs tiea)
    {
        switch (_state)
        {
            case EditorState.SetSize:
                _setSize.UpdateInput(sender, tiea);
                break;
            case EditorState.SaveLevel:
                _saveLevel.UpdateInput(sender, tiea);
                break;
        }
    }
}
