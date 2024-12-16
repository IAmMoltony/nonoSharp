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
    private string _levelName; // when editing existing level

    public Editor()
    {
        _editingExistingLevel = false;
        _levelName = "";
        _state = EditorState.SetSize;
        _setSize = new();
        _main = new();
        _saveLevel = new();
    }

    public Editor(string levelName)
    {
        _editingExistingLevel = true;
        _levelName = levelName;
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
                    _main.EnableAutoSaveTimer(true);
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
                {
                    _main.EnableAutoSaveTimer(false);
                    if (_editingExistingLevel)
                    {
                        BoardSaver.SaveBoard(_main.Board, _levelName, _main.MaxHintsBox.GetNumberValue());
                        LevelSelect levelSelect = new();
                        levelSelect.FindLevels();
                        return levelSelect; // TODO just find the levels in the constructor???
                    }
                    else
                        _state = EditorState.SaveLevel;
                }
                if (_main.BackButton.IsClicked)
                {
                    _main.EnableAutoSaveTimer(false);
                    if (_editingExistingLevel)
                    {
                        LevelSelect levelSelect = new();
                        levelSelect.FindLevels();
                        return levelSelect;
                    }
                    else
                    {
                        _main.AutoSave();
                        return new MainMenu();
                    }
                }
                break;
            case EditorState.SaveLevel:
                _saveLevel.Update(mouse, mouseOld, kb, kbOld, _main.Board, _main.MaxHintsBox.GetNumberValue());
                if (_saveLevel.OKButton.IsClicked)
                    return new MainMenu();
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

    public void AutoSave()
    {
        if (_state == EditorState.Editor)
        {
            _main.AutoSave();
        }
    }
}
