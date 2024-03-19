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
    private bool _paused = false;

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

    public void Update(MouseState mouse, MouseState mouseOld, KeyboardState kb, KeyboardState kbOld, GraphicsDevice graphDev)
    {
        if (!_paused)
        {
            _board.Update(mouse, mouseOld, graphDev);
            if (_board.IsSolved)
                _solveTimeTick = false; 
        }

        // pause button
        if (!_board.IsSolved && ((kb.IsKeyDown(Keys.Space) && !kbOld.IsKeyDown(Keys.Space)) || (kb.IsKeyDown(Keys.Escape) && !kbOld.IsKeyDown(Keys.Escape))))
            pause(); 
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

        if (_paused)
        {
            RectRenderer.DrawRect(new(0, 0, sprBatch.GraphicsDevice.Viewport.Bounds.Width, sprBatch.GraphicsDevice.Viewport.Bounds.Height), new(0.0f, 0.3f, 0.0f, 0.6f), sprBatch);
            TextRenderer.DrawText(sprBatch, "notosans", 10, 10, "Paused", Color.White);
            TextRenderer.DrawText(sprBatch, "notosans", 10, 80, 0.6f, "Press Space or Esacpe to unpause", Color.White);
        }
    }

    public void StopSolveTimeThread()
    {
        Log.Logger.Information("Stopping solve time thread");
        _solveTimeThreadRunning = false;
        _solveTimeThread.Join();
    }

    private void pause()
    {
        Log.Logger.Information("Pausing the game rn");
        if (_paused)
        {
            _paused = false;
            _solveTimeTick = true; 
        }
        else
        {
            _paused = true;
            _solveTimeTick = false;
        }
    }
}
