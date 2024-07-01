using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace NonoSharp;

public class PerformanceInfo
{
    private FPSCounter _fpsCounter;
    private Process _gameProcess;

    public PerformanceInfo()
    {
        _fpsCounter = new();
        _gameProcess = Process.GetCurrentProcess();
    }

    public void UpdateFPS(GameTime gameTime)
    {
        _fpsCounter.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
    }

    public void UpdatePerformanceInfo()
    {
        if (_fpsCounter.TotalFrames % 60 == 0)
            _gameProcess.Refresh();
    }

    public void Draw(SpriteBatch sprBatch)
    {
        TextRenderer.DrawText(sprBatch, "DefaultFont", 10, sprBatch.GraphicsDevice.Viewport.Bounds.Height - 26, 0.33f, $"{Math.Round(_fpsCounter.CurrentFPS)} fps, {Math.Round(_fpsCounter.AverageFPS)} avg", Color.LightGray); // FPS
        TextRenderer.DrawText(sprBatch, "DefaultFont", 10, sprBatch.GraphicsDevice.Viewport.Bounds.Height - 42, 0.33f, $"mem: {Math.Round(((float)_gameProcess.WorkingSet64 / 1024 / 1024), 2)}M (peak {Math.Round(((float)_gameProcess.PeakWorkingSet64 / 1024 / 1024), 2)}M)", Color.LightGray); // Memory usage (current and peak)
    }
}
