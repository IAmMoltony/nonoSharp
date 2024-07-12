using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NonoSharp.UI;
using Serilog;
using System;
using System.Threading;

namespace NonoSharp;

public class PlayState : IGameState
{
    private static float _solveTime;
    private static Thread _solveTimeThread;
    private static bool _solveTimeThreadRunning = true;
    private static bool _solveTimeTick = true;

    private readonly Board _board;
    private bool _paused;
    private readonly Button _solvedContinueButton;
    private readonly Button _pauseBackButton;
    private readonly Button _pauseRestartButton;
    private int _usedHints;
    private int _hintsTextRedness;

    private static void solveTimeTick()
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
        _solvedContinueButton = new(30, 194, 0, 50, StringManager.GetString("continue"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), true);
        _pauseBackButton = new(10, 130, 0, 50, StringManager.GetString("back"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), true);
        _pauseRestartButton = new(10, 190, 0, 50, StringManager.GetString("restart"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), true);
        _usedHints = 0;
        _hintsTextRedness = 255;

        _solveTimeThread = new(solveTimeTick);
        _solveTimeTick = false;
        _solveTimeThread.Start();
    }

    public IGameState? Update(MouseState mouse, MouseState mouseOld, KeyboardState kb, KeyboardState kbOld, GraphicsDevice graphDev, ref LevelMetadata levelMetadata, bool hasFocus)
    {
        _hintsTextRedness = Math.Min(255, _hintsTextRedness + 10);

        // continue button
        if (_board.IsSolved)
        {
            _solvedContinueButton.Update(mouse, mouseOld, kb, kbOld);

            if (_solvedContinueButton.IsClicked)
            {
                leaveGame();
                var levelSelect = new LevelSelect();
                levelSelect.FindLevels();
                return levelSelect;
            }
        }

        if (!_paused)
        {
            _board.Update(mouse, mouseOld, kb, kbOld, graphDev);
            if (_board.IsSolved)
                _solveTimeTick = false;
            else
            {
                // undo button
                if (kb.IsKeyDown(Keys.Z) && !kbOld.IsKeyDown(Keys.Z))
                    _board.RestoreState();

                // hint button
                if (kb.IsKeyDown(Keys.H) && !kbOld.IsKeyDown(Keys.H) && Settings.GetBool("enableHints"))
                {
                    bool canHint = _board.maxHints <= -1 || _board.maxHints > _usedHints;
                    if (canHint)
                    {
                        _usedHints++;
                        Log.Logger.Information($"Doing a hint, used hints: {_usedHints}/{_board.maxHints}");
                        _board.Hint();
                    }
                    else
                    {
                        _hintsTextRedness = 0;
                    }
                }
            }
        }
        else
        {
            _pauseBackButton.Update(mouse, mouseOld, kb, kbOld);
            if (_pauseBackButton.IsClicked)
            {
                leaveGame();
                var levelSelect = new LevelSelect();
                levelSelect.FindLevels();
                return levelSelect;
            }

            _pauseRestartButton.Update(mouse, mouseOld, kb, kbOld);
            if (_pauseRestartButton.IsClicked)
            {
                restart(graphDev);
            }
        }

        // pause button
        if (!_board.IsSolved && ((kb.IsKeyDown(Keys.Space) && !kbOld.IsKeyDown(Keys.Space)) || (kb.IsKeyDown(Keys.Escape) && !kbOld.IsKeyDown(Keys.Escape))))
            pause(graphDev);

        // when window inactive, force pause
        if (!hasFocus && !_paused && !_board.IsSolved)
            pause(graphDev);

        return null;
    }

    public void Load(string levelPath)
    {
        Log.Logger.Information($"Loading level {levelPath}");
        _board.Load(levelPath);
        _solveTimeTick = true;
    }

    public void Draw(SpriteBatch sprBatch)
    {
        Color hintsTextColor = new(255, _hintsTextRedness, _hintsTextRedness);

        // render time and hints if the board isn't solved
        if (!_board.IsSolved)
        {
            _board.Draw(sprBatch);
            TextRenderer.DrawText(sprBatch, "DefaultFont", 10, 10, 0.5f, string.Format(StringManager.GetString("solveTime"), Math.Round(_solveTime / 1000, 2)), Color.White);
            if (_board.maxHints > -1)
            {
                TextRenderer.DrawText(sprBatch, "DefaultFont", 10, 40, 0.5f, string.Format(StringManager.GetString("hintsOutOf"), _usedHints, _board.maxHints), hintsTextColor);
            }
            else
            {
                TextRenderer.DrawText(sprBatch, "DefaultFont", 10, 40, 0.5f, string.Format(StringManager.GetString("hints"), _usedHints), hintsTextColor);
            }
        }

        if (_board.IsSolved)
        {
            // draw half transparent rectangle across the whole screen
            RectRenderer.DrawRect(new(0, 0, sprBatch.GraphicsDevice.Viewport.Bounds.Width, sprBatch.GraphicsDevice.Viewport.Bounds.Height), new(Settings.GetDarkAccentColor(), 0.2f), sprBatch);

            // draw the board, on top of the rect
            _board.Draw(sprBatch);

            // render the text
            TextRenderer.DrawText(sprBatch, "DefaultFont", 30, 30, 1.0f, StringManager.GetString("solved"), Color.White);
            TextRenderer.DrawText(sprBatch, "DefaultFont", 30, 90, 1.0f, string.Format(StringManager.GetString("inSolveTime"), Math.Round(_solveTime / 1000, 2)), Color.White);
            if (_usedHints > 0)
            {
                // draw how many hints used
                if (_usedHints > 1)
                    TextRenderer.DrawText(sprBatch, "DefaultFont", 30, 146, 0.7f, string.Format(StringManager.GetString("withHints"), _usedHints), Color.Yellow);
                else
                    TextRenderer.DrawText(sprBatch, "DefaultFont", 30, 146, 0.7f, StringManager.GetString("withHints1"), Color.Yellow);
            }

            // draw the continue button
            _solvedContinueButton.Draw(sprBatch);
        }

        if (_paused)
        {
            RectRenderer.DrawRect(new(0, 0, sprBatch.GraphicsDevice.Viewport.Bounds.Width, sprBatch.GraphicsDevice.Viewport.Bounds.Height), new(Settings.GetDarkAccentColor(), 0.2f), sprBatch);
            TextRenderer.DrawText(sprBatch, "DefaultFont", 10, 10, StringManager.GetString("paused"), Color.White);
            TextRenderer.DrawText(sprBatch, "DefaultFont", 10, 80, 0.6f, StringManager.GetString("pauseTip"), Color.White);
            _pauseBackButton.Draw(sprBatch);
            _pauseRestartButton.Draw(sprBatch);
        }
    }

    public static void StopSolveTimeThread()
    {
        Log.Logger.Information("Stopping solve time thread");
        _solveTimeThreadRunning = false;
        _solveTimeThread?.Join();
    }

    private void pause(GraphicsDevice graphDev)
    {
        Log.Logger.Information("Pausing the game rn");
        if (_paused)
        {
            _paused = false;
            _solveTimeTick = true;
            Mouse.SetPosition(graphDev.Viewport.Bounds.Width / 2, graphDev.Viewport.Bounds.Height / 2);
        }
        else
        {
            _paused = true;
            _solveTimeTick = false;
        }
    }

    private void leaveGame()
    {
        _solveTimeTick = false;
        _solveTime = 0f;
        _paused = false;
        _board.Reset();
        _usedHints = 0;
    }

    private void restart(GraphicsDevice graphDev)
    {
        _board.Clear();
        _solveTime = 0f;
        _solveTimeTick = true;
        _usedHints = 0;
        _paused = false;
        Mouse.SetPosition(graphDev.Viewport.Bounds.Width / 2, graphDev.Viewport.Bounds.Height / 2);
    }
}
