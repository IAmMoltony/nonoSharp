using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NonoSharp.UI;

namespace NonoSharp.Editor;

public class EditorMain
{
    public Button SaveButton { get; private set; }
    public Button ResetButton { get; private set; }
    public EditorBoard Board { get; private set; }

    public EditorMain()
    {
        Board = new();
        SaveButton = new(10, 10, 0, 45, StringManager.GetString("save"), Color.DarkGreen, Color.Green, Keys.S, true);
        ResetButton = new(10, 65, 0, 45, StringManager.GetString("reset"), Color.DarkGreen, Color.Green, Keys.R, true);
    }

    public void Update(MouseState mouse, MouseState mouseOld, KeyboardState kb, KeyboardState kbOld, GraphicsDevice graphDev)
    {
        Board.Update(mouse, mouseOld, graphDev);
        SaveButton.Update(mouse, mouseOld, kb, kbOld);
        ResetButton.Update(mouse, mouseOld, kb, kbOld);

        if (ResetButton.IsClicked)
        {
            int size = Board.size;
            Board.Reset();
            Board.Make(size);
        }
    }

    public void Draw(SpriteBatch sprBatch)
    {
        Board.Draw(sprBatch);
        SaveButton.Draw(sprBatch);
        ResetButton.Draw(sprBatch);
    }
}
