namespace NonoSharp;

public struct LevelMetadata
{
    public string name;
    public int size;

    public LevelMetadata(string name, int size)
    {
        this.name = name;
        this.size = size;
    }

    public LevelMetadata()
    {
        name = "";
        size = 0;
    }

    public override string ToString()
    {
        return $"{name} ({size}x{size})";
    }
}