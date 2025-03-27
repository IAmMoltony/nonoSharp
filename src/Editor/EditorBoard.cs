using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Serilog;
using System;
using System.IO;
using System.Collections.Generic;

namespace NonoSharp.Editor;

public class EditorBoard : Board
{
    public bool TestMode { get; private set; }
    public bool TestModeSolved
    {
        get
        {
            return _testBoard.IsSolved && TestMode;
        }
    }

    private Board _testBoard;

    public EditorBoard() : base()
    {
        _testBoard = new();
    }

    public void Make(int size)
    {
        this.size = size;
        Clues = new(size);
        MakeTiles();
        MakeSolution();
    }

    public void Make(string levelName)
    {
        tiles = Load(Path.Combine(BoardSaver.GetLevelSavePath(), $"{levelName}.nono"));
        makeClues();
    }

    public void Undo()
    {
        if (!TestMode)
        {
            RestoreState();
            makeClues();
        }
    }

    public void EnterTestMode()
    {
        Log.Logger.Information("Entering level test mode");
        TestMode = true;
        if (tiles == null)
        {
            Log.Logger.Error("tiles is null when entering test mode");
        }
        else
        {
            _testBoard = new(tiles);
        }
    }

    public void ExitTestMode()
    {
        TestMode = false;
    }

    public override void Draw(SpriteBatch sprBatch)
    {
        if (TestMode)
            _testBoard.Draw(sprBatch);
        else
            base.Draw(sprBatch);
    }

    public override void Update(MouseState mouseState, MouseState mouseStateOld, KeyboardState kb, KeyboardState kbOld, GraphicsDevice graphDev)
    {
        if (TestMode)
            _testBoard.Update(mouseState, mouseStateOld, kb, kbOld, graphDev);
        else
            base.Update(mouseState, mouseStateOld, kb, kbOld, graphDev);
    }

    protected override void DoMouseInput(bool isLeftButton, ref Tile tile)
    {
        if (!TestMode)
        {
            tile.LeftClick();
            makeClues();
            Console.WriteLine(BoardChecker.CheckBoard(tiles, Clues));
            Tile.PrintTileArray(tiles, size);
        }
    }

    protected override void CheckSolution()
    {
    }

    private void makeClues()
    {
        Clues = new(this, false);
    }
}
