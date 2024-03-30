using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NonoSharp.UI;

namespace NonoSharp.Editor;

public class EditorMain
{
    public Button SaveButton { get; private set; }
    public EditorBoard Board { get; private set; }

    public EditorMain()
    {
        Board = new();
        SaveButton = new(10, 10, 135, 45, StringManager.GetString("save"), Color.DarkGreen, Color.Green);
    }

    public void Update(MouseState mouse, MouseState mouseOld, KeyboardState kb, KeyboardState kbOld, GraphicsDevice graphDev)
    {
        Board.Update(mouse, mouseOld, graphDev);
        SaveButton.Update(mouse, mouseOld, kb, kbOld);
    }

    public void Draw(SpriteBatch sprBatch)
    {
        Board.Draw(sprBatch);
        SaveButton.Draw(sprBatch);
    }
}
