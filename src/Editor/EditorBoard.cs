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
        MakeTilesAndSolution();
    }

    protected override void DrawClues(int boardX, int boardY, SpriteBatch batch)
    {
    }

    protected override void DoMouseInput(bool isLeftButton, ref Tile tile)
    {
        tile.LeftClick();
    }

    protected override void CheckSolution()
    {
    }
}