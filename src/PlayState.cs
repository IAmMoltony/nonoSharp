using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Serilog;
using NonoSharp.UI;

namespace NonoSharp;

public class PlayState
{
    private static float _solveTime = 0;
    private static Thread _solveTimeThread;
    private static bool _solveTimeThreadRunning = true;
    private static bool _solveTimeTick = true;

    private Board _board;
    private bool _paused;
    private Button _solvedContinueButton;

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
        _paused = false;
        _solvedContinueButton = new(0, 0, 130, 50, "Continue", Color.DarkGreen, Color.Green);

        _solveTimeThread = new(SolveTimeTick);
        _solveTimeTick = false;
        _solveTimeThread.Start();
    }

    public void Update(MouseState mouse, MouseState mouseOld, KeyboardState kb, KeyboardState kbOld, GraphicsDevice graphDev, out bool leave)
    {
        leave = false;

        if (!_paused)
        {
            _board.Update(mouse, mouseOld, graphDev);
            if (_board.IsSolved)
                _solveTimeTick = false;
        }

        // pause button
        if (!_board.IsSolved && ((kb.IsKeyDown(Keys.Space) && !kbOld.IsKeyDown(Keys.Space)) || (kb.IsKeyDown(Keys.Escape) && !kbOld.IsKeyDown(Keys.Escape))))
            pause();

        // continue button
        if (_board.IsSolved)
        {
            _solvedContinueButton.x = graphDev.Viewport.Bounds.Width / 2 - _solvedContinueButton.width / 2;
            _solvedContinueButton.y = graphDev.Viewport.Bounds.Height / 2 + 40;
        }

        _solvedContinueButton.Update(mouse, mouseOld, kb, kbOld);

        if (_solvedContinueButton.IsClicked)
        {
            _solveTimeTick = false;
            _solveTime = 0.0f;
            _board.Reset();
            leave = true;
        }
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

        // render time in format "Time: x.xx s"
        TextRenderer.DrawText(sprBatch, "notosans", 10, 10, 0.6f, $"Time: {Math.Round(_solveTime / 1000, 2)} s", _board.IsSolved ? Color.Lime : Color.White);

        if (_board.IsSolved)
        {
            // draw green half transparent rectangle across the whole screen
            RectRenderer.DrawRect(new(0, 0, sprBatch.GraphicsDevice.Viewport.Bounds.Width, sprBatch.GraphicsDevice.Viewport.Bounds.Height), new(0.0f, 0.2f, 0.0f, 0.3f), sprBatch);

            // solved text: 100 pixels above center of the screen, centered horizontally
            Rectangle solvedTextRect = new(0, sprBatch.GraphicsDevice.Viewport.Bounds.Height / 2 - 100, sprBatch.GraphicsDevice.Viewport.Bounds.Width, 1);

            // "how long the user took to solve" text: 50 pixels bellow solved text, everything else is the same
            Rectangle inTimeTextRect = new(0, solvedTextRect.Y + 50, sprBatch.GraphicsDevice.Viewport.Bounds.Width, 1);

            // render the text
            TextRenderer.DrawTextCenter(sprBatch, "notosans", 0, 0, 1.0f, "Solved!", Color.White, solvedTextRect);
            TextRenderer.DrawTextCenter(sprBatch, "notosans", 0, 0, 1.0f, $"in {Math.Round(_solveTime / 1000, 2)} seconds", Color.White, inTimeTextRect);

            // draw the continue button
            _solvedContinueButton.Draw(sprBatch);
        }

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
