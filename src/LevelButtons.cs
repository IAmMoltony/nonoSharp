using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using NonoSharp.UI;

namespace NonoSharp;

public struct LevelButtons
{
    public Button? playButton;
    public Button? deleteButton;
    public Button? renameButton;
    public Button? editButton;

    public readonly void SetY(int y)
    {
        if (playButton != null)
            playButton.y = y;
        if (deleteButton != null)
            deleteButton.y = y;
        if (renameButton != null)
            renameButton.y = y;
        if (editButton != null)
            editButton.y = y;
    }

    public LevelButtons()
    {
        playButton = null;
        deleteButton = null;
        renameButton = null;
        editButton = null;
    }

    public readonly void Draw(SpriteBatch sprBatch)
    {
        playButton?.Draw(sprBatch);
        deleteButton?.Draw(sprBatch);
        renameButton?.Draw(sprBatch);
        editButton?.Draw(sprBatch);
    }

    public readonly void Update(MouseState mouse, MouseState mouseOld, KeyboardState keyboard, KeyboardState keyboardOld)
    {
        playButton?.Update(mouse, mouseOld, keyboard, keyboardOld);
        deleteButton?.Update(mouse, mouseOld, keyboard, keyboardOld);
        renameButton?.Update(mouse, mouseOld, keyboard, keyboardOld);
        editButton?.Update(mouse, mouseOld, keyboard, keyboardOld);
    }
}
