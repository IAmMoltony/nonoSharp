using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Serilog;

namespace NonoSharp;

public class PlayState
{
    private Board _board;
    private static float _solveTime = 0;
    private static Thread _solveTimeThread;
    private static bool _solveTimeThreadRunning = true;
    private static bool _solveTimeTick = true;

    private static void SolveTimeTick()
    {
        while (_solveTimeThreadRunning)
        {
            if (_solveTimeTick)
                _solveTime++;
            Thread.Sleep(1);
        } 
    }


    public PlayState()
    {
        _board = new();
        _solveTimeThread = new(SolveTimeTick);
        _solveTimeTick = false;
        _solveTimeThread.Start();
    }

    public void Update(MouseState mouse, MouseState mouseOld, GraphicsDevice graphDev)
    {
        _board.Update(mouse, mouseOld, graphDev);
        if (_board.IsSolved)
           _solveTimeTick = false; 
    }

    public void Load(string levelPath)
    {
        Log.Logger.Information($"Loading level {levelPath}");
        _board.Load(levelPath);
        _solveTimeTick = true;
    }

    public void Draw(SpriteBatch sprBatch)
    {
        _board.Draw(sprBatch);
        TextRenderer.DrawText(sprBatch, "notosans", 10, 10, 0.6f, $"Time: {Math.Round(_solveTime / 1000, 2)} s", _board.IsSolved ? Color.Lime : Color.White);
    }

    public void StopSolveTimeThread()
    {
        Log.Logger.Information("Stopping solve time thread");
        _solveTimeThreadRunning = false;
        _solveTimeThread.Join();
    }
}
