﻿using Microsoft.Xna.Framework;
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
        SaveButton = new(10, 10, 0, 45, StringManager.GetString("save"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), Keys.S, true);
        ResetButton = new(10, 65, 0, 45, StringManager.GetString("reset"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), Keys.R, true);
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

        // undo button
        if (kb.IsKeyDown(Keys.Z) && !kbOld.IsKeyDown(Keys.Z))
            Board.Undo();
    }

    public void Draw(SpriteBatch sprBatch)
    {
        Board.Draw(sprBatch);
        SaveButton.Draw(sprBatch);
        ResetButton.Draw(sprBatch);
    }
}
