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

    protected override void DoMouseInput(bool isLeftButton, ref Tile tile)
    {
        tile.LeftClick();
        clues = new(this, false);
    }

    protected override void CheckSolution()
    {
    }
}
