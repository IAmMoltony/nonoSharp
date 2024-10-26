using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace NonoSharp.Editor;

public enum EditorState
{
    SetSize,
    Editor,
    SaveLevel,
}

public class Editor : IGameState
{
    private EditorState _state;
    // TODO do the same thing as done with regular game states but for editor
    private SetSizeState _setSize;
    private readonly EditorMain _main;
    private SaveLevelState _saveLevel;
    private bool _editingExistingLevel;

    public Editor()
    {
        _editingExistingLevel = false;
        _state = EditorState.SetSize;
        _setSize = new();
        _main = new();
        _saveLevel = new();
    }

    public Editor(string levelName)
    {
        _editingExistingLevel = true;
        _state = EditorState.Editor;
        _setSize = new();
        _main = new(levelName);
        _saveLevel = new();
    }

    public IGameState? Update(MouseState mouse, MouseState mouseOld, KeyboardState kb, KeyboardState kbOld, GraphicsDevice graphDev, ref LevelMetadata levelMetadata, bool hasFocus)
    {
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
                    return new MainMenu();
                }
                break;
            case EditorState.Editor:
                _main.Update(mouse, mouseOld, kb, kbOld, graphDev);
                if (_main.SaveButton.IsClicked)
                    _state = EditorState.SaveLevel;
                if (_main.BackButton.IsClicked)
                {
                    if (_editingExistingLevel)
                    {
                        LevelSelect levelSelect = new();
                        levelSelect.FindLevels();
                        return levelSelect;
                    }
                    else
                        return new MainMenu();
                }
                break;
            case EditorState.SaveLevel:
                _saveLevel.Update(mouse, mouseOld, kb, kbOld, _main.Board, _main.MaxHintsBox.GetNumberValue());
                if (_saveLevel.OKButton.IsClicked)
                {
                    _saveLevel = new();
                    return new MainMenu();
                }
                if (_saveLevel.BackButton.IsClicked)
                    _state = EditorState.Editor;
                break;
        }

        return null;
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

    public void UpdateInput(TextInputEventArgs tiea)
    {
        switch (_state)
        {
            case EditorState.SetSize:
                _setSize.UpdateInput(tiea);
                break;
            case EditorState.SaveLevel:
                _saveLevel.UpdateInput(tiea);
                break;
            case EditorState.Editor:
                _main.UpdateInput(tiea);
                break;
        }
    }
}
