using Microsoft.Xna.Framework.Graphics;

namespace NonoSharp.Editor;

public class EditorBoard : Board
{
    public EditorBoard() : base()
    {
    }

    public void Make(int size)
    {
        this.size = size;
        clues = new(size);
        MakeTilesAndSolution();
    }

    public void Undo()
    {
        RestoreState();
        makeClues();
    }

    protected override void DoMouseInput(bool isLeftButton, ref Tile tile)
    {
        tile.LeftClick();
        makeClues();
    }

    protected override void CheckSolution()
    {
    }

    private void makeClues()
    {
        clues = new(this, false);
    }
}
