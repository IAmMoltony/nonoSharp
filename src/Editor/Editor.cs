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
    private SaveLevelState _saveLevel;

    private EditorBoard _board;
    private UI.Button _saveButton;

    public Editor()
    {
        _state = EditorState.SetSize;
        _setSize = new();
        _board = new();
        _saveLevel = new();
        _saveButton = new(10, 10, 90, 45, "Save", Color.DarkGreen, Color.Green);
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
                    _board.Make(_setSize.GetSize());
                }
                if (_setSize.BackButton.IsClicked)
                {
                    _state = EditorState.SetSize;
                    _setSize = new();
                    newState = GameState.MainMenu;
                }
                break;
            case EditorState.Editor:
                _board.Update(mouse, mouseOld, graphDev);
                _saveButton.Update(mouse, mouseOld, kb, kbOld);

                if (_saveButton.IsClicked)
                    _state = EditorState.SaveLevel;
                break;
            case EditorState.SaveLevel:
                _saveLevel.Update(mouse, mouseOld, kb, kbOld, _board);
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
                _saveButton.Draw(sprBatch);
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
