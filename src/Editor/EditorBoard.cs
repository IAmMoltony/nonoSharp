namespace NonoSharp.Editor;

public class EditorBoard : Board
{
    public EditorBoard() : base()
    {
        drawClues = false;
        checkSolution = false;
        canRightClick = false;
    }

    public void Make(int size)
    {
        this.size = size;
        MakeTilesAndSolution();
    }
}