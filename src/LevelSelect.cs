using System;
using System.IO;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NonoSharp.UI;
using Serilog;

namespace NonoSharp;

public class LevelSelect
{
    private Tuple<string, Button>[] _levels;
    private int _scrollOffset;

    public LevelSelect()
    {
        _scrollOffset = 0;
    }

    public void FindLevels()
    {
        Stopwatch stopwatch = new();
        stopwatch.Start();

        Log.Logger.Information("Finding levels");
        string levelsDir = AppDomain.CurrentDomain.BaseDirectory + "/Content/Levels";
        DirectoryInfo dirInfo = new DirectoryInfo(levelsDir);
        FileInfo[] files = dirInfo.GetFiles("*.nono");

        _levels = new Tuple<string, Button>[files.Length];
        for (int i = 0; i < _levels.Length; i++)
        {
            _levels[i] = new(Path.GetFileNameWithoutExtension(files[i].Name), new(10, 110 + 120 * i + 40, 67, 40, "Play", Color.DarkGreen, Color.Green));
            Log.Logger.Information($"Found level: {_levels[i].Item1}");
        }

        stopwatch.Stop();
        Log.Logger.Information($"Found levels in {stopwatch.ElapsedMilliseconds} ms");
    }

    public void Draw(SpriteBatch sprBatch, GraphicsDevice graphDev)
    {
        for (int i = 0; i < _levels.Length; i++)
        {
            string name = _levels[i].Item1;
            TextRenderer.DrawText(sprBatch, "notosans", 10, 110 + 120 * i + _scrollOffset, 0.56f, name, Color.White);
            _levels[i].Item2.Draw(sprBatch);
        }

        Rectangle nameRect = new(0, 15, graphDev.Viewport.Bounds.Width, 100);
        Rectangle nameBackgroundRect = new(0, 0, graphDev.Viewport.Bounds.Width, 100);
        RectRenderer.DrawRect(nameBackgroundRect, Color.Black, sprBatch);
        TextRenderer.DrawTextCenter(sprBatch, "notosans", 0, 0, 0.9f, "Select level", Color.White, nameRect);
    }

    public void Update(MouseState mouse, MouseState mouseOld, KeyboardState kb, KeyboardState kbOld, ref bool shouldStart, ref string levelName)
    {
        if (mouse.ScrollWheelValue > mouseOld.ScrollWheelValue)
            _scrollOffset += 25;
        else if (mouse.ScrollWheelValue < mouseOld.ScrollWheelValue)
            _scrollOffset -= 25;
        if (_scrollOffset > 0)
            _scrollOffset = 0;
        if (_scrollOffset < -(120 * _levels.Length - 230))
            _scrollOffset = -(120 * _levels.Length - 230);

        for (int i = 0; i < _levels.Length; i++)
        {
            var level = _levels[i];
            level.Item2.Update(mouse, mouseOld, kb, kbOld);
            if (level.Item2.IsClicked)
            {
                Log.Logger.Information($"Clicked on button for level {level.Item1}");
                shouldStart = true;
                levelName = level.Item1;
            }
            level.Item2.y = 110 + 120 * i + 40 + _scrollOffset;
        }
    }
}
