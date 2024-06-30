namespace NonoSharp;

public enum ReplayMoveType
{
    LeftClick,
    RightClick
}

public struct ReplayMove
{
    public ReplayMoveType type;
    public int tileX;
    public int tileY;

    public ReplayMove(ReplayMoveType type, int tileX, int tileY)
    {
        this.type = type;
        this.tileX = tileX;
        this.tileY = tileY;
    }
}
