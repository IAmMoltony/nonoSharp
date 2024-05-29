using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NonoSharp.UI;

namespace NonoSharp.Editor;

public class EditorMain
{
    public Button SaveButton { get; private set; }
    public Button ResetButton { get; private set; }
    public Button BackButton { get; private set; }
    public NumberTextBox MaxHintsBox { get; private set; }
    public EditorBoard Board { get; private set; }

    public EditorMain()
    {
        Board = new();
        SaveButton = new(10, 10, 0, 45, StringManager.GetString("save"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), Keys.S, true);
        ResetButton = new(10, 65, 0, 45, StringManager.GetString("reset"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), Keys.R, true);
        BackButton = new(10, 120, 0, 45, StringManager.GetString("back"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), Keys.Escape, true);
        MaxHintsBox = new(10, 0, 195, Color.Gray, Color.DarkGray, Color.White, Color.White, Color.DarkGray, Color.LightGray, StringManager.GetString("maxHintsPlaceholder"));
    }

    public void Update(MouseState mouse, MouseState mouseOld, KeyboardState kb, KeyboardState kbOld, GraphicsDevice graphDev)
    {
        Board.Update(mouse, mouseOld, graphDev);
        SaveButton.Update(mouse, mouseOld, kb, kbOld);
        ResetButton.Update(mouse, mouseOld, kb, kbOld);
        BackButton.Update(mouse, mouseOld, kb, kbOld);
        MaxHintsBox.Update(mouse, mouseOld, kb, kbOld);

        MaxHintsBox.y = graphDev.Viewport.Bounds.Height - 10 - TextBox.Height;

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

    public void UpdateInput(object sender, TextInputEventArgs tiea)
    {
        MaxHintsBox.UpdateInput(sender, tiea);
    }

    public void Draw(SpriteBatch sprBatch)
    {
        Board.Draw(sprBatch);
        SaveButton.Draw(sprBatch);
        ResetButton.Draw(sprBatch);
        BackButton.Draw(sprBatch);
        MaxHintsBox.Draw(sprBatch);
        TextRenderer.DrawText(sprBatch, "DefaultFont", 10, MaxHintsBox.y - TextBox.Height - 14, 0.5f, StringManager.GetString("maxHints"), Color.White);
    }
}
