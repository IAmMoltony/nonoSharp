using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using NonoSharp.UI;

namespace NonoSharp;

public struct LevelButtons
{
    public Button playButton;
    public Button deleteButton;

    public readonly void SetY(int value)
    {
        playButton.y = value;
        deleteButton.y = value;
    }

    public LevelButtons()
    {
        playButton = null;
        deleteButton = null;
    }

    public readonly void Draw(SpriteBatch sprBatch)
    {
        playButton.Draw(sprBatch);
        deleteButton.Draw(sprBatch);
    }

    public readonly void Update(MouseState mouse, MouseState mouseOld, KeyboardState keyboard, KeyboardState keyboardOld)
    {
        playButton.Update(mouse, mouseOld, keyboard, keyboardOld);
        deleteButton.Update(mouse, mouseOld, keyboard, keyboardOld);
    }
}
