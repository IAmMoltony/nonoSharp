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
    public Button TestButton { get; private set; }
    public Button TestBackButton { get; private set; }
    public Button TestResetButton { get; private set; }
    public NumberTextBox MaxHintsBox { get; private set; }
    public EditorBoard Board { get; private set; }

    public EditorMain()
    {
        Board = new();
        SaveButton = new(10, 10, 0, 45, StringManager.GetString("save"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), Keys.S, true);
        ResetButton = new(10, 65, 0, 45, StringManager.GetString("reset"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), Keys.R, true);
        BackButton = new(10, 120, 0, 45, StringManager.GetString("back"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), Keys.Escape, true);
        TestButton = new(10, 0, 0, 45, StringManager.GetString("test"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), Keys.T, true);
        TestBackButton = new(10, 10, 0, 45, StringManager.GetString("back"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), Keys.Escape, true);
        TestResetButton = new(0, 0, 0, 45, StringManager.GetString("restart"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), Keys.R, true);
        MaxHintsBox = new(10, 0, 195, Color.Gray, Color.DarkGray, Color.White, Color.White, Color.DarkGray, Color.LightGray, StringManager.GetString("maxHintsPlaceholder"));
    }

    public void Update(MouseState mouse, MouseState mouseOld, KeyboardState kb, KeyboardState kbOld, GraphicsDevice graphDev)
    {
        Board.Update(mouse, mouseOld, kb, kbOld, graphDev);
 
        if (Board.TestMode)
        {
            TestBackButton.Update(mouse, mouseOld, kb, kbOld);

            if (TestBackButton.IsClicked)
                Board.ExitTestMode();

            if (Board.TestModeSolved)
            {
                TestResetButton.x = (graphDev.Viewport.Bounds.Width / 2) - (TestResetButton.width / 2);
                TestResetButton.y = graphDev.Viewport.Bounds.Height - (graphDev.Viewport.Bounds.Height / 6);
                TestResetButton.Update(mouse, mouseOld, kb, kbOld);
                if (TestResetButton.IsClicked)
                    Board.EnterTestMode();
            }
        }
        else
        {
            SaveButton.Update(mouse, mouseOld, kb, kbOld);
            ResetButton.Update(mouse, mouseOld, kb, kbOld);
            BackButton.Update(mouse, mouseOld, kb, kbOld);
            TestButton.Update(mouse, mouseOld, kb, kbOld);
            MaxHintsBox.Update(mouse, mouseOld, kb, kbOld);

            MaxHintsBox.y = graphDev.Viewport.Bounds.Height - 10 - TextBox.Height;
            TestButton.y = MaxHintsBox.y - 40 - TestButton.height;

            if (TestButton.IsClicked)
                Board.EnterTestMode();

            if (ResetButton.IsClicked)
            {
                int size = Board.size;
                Board.Reset();
                Board.Make(size);
            }
        }

        // undo button
        if (kb.IsKeyDown(Keys.Z) && !kbOld.IsKeyDown(Keys.Z))
            Board.Undo();
    }

    public void UpdateInput(TextInputEventArgs tiea)
    {
        MaxHintsBox.UpdateInput(tiea);
    }

    public void Draw(SpriteBatch sprBatch)
    {
        Board.Draw(sprBatch);

        if (Board.TestMode)
        {
            TestBackButton.Draw(sprBatch);

            if (Board.TestModeSolved)
            {
                TextRenderer.DrawTextCenter(sprBatch, "DefaultFont", 1.0f, StringManager.GetString("solved"), Color.White, new(0, 0, sprBatch.GraphicsDevice.Viewport.Bounds.Width, sprBatch.GraphicsDevice.Viewport.Bounds.Height / 6));
                TestResetButton.Draw(sprBatch);
            }
        }
        else
        {
            SaveButton.Draw(sprBatch);
            ResetButton.Draw(sprBatch);
            BackButton.Draw(sprBatch);
            TestButton.Draw(sprBatch);
            MaxHintsBox.Draw(sprBatch);
            TextRenderer.DrawText(sprBatch, "DefaultFont", 10, MaxHintsBox.y - TextBox.Height - 4, 0.5f, StringManager.GetString("maxHints"), Color.White);
        }
    }
}
