using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NonoSharp.UI;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NonoSharp;

public class LevelSelect
{
    private List<Tuple<LevelMetadata, Button>> _levels;
    private int _scrollOffsetGoal;
    private float _scrollOffset;
    private Button _backButton;

    public LevelSelect()
    {
        _scrollOffsetGoal = 0;
        _scrollOffset = 0;
        _backButton = new(10, 10, 0, 40, StringManager.GetString("back"), Color.DarkGreen, Color.Green, Keys.Escape, true);
    }

    public void FindLevels()
    {
        _levels = new();

        Stopwatch stopwatch = new();
        stopwatch.Start();

        // Find levels
        Log.Logger.Information("Finding levels");
        LevelList list = new();
        list.FindLevels();

        // copy over the list into the internal list and slap in buttons
        for (int i = 0; i < list.Count(); i++)
        {
            _levels.Add(new(list[i], new(15, 110 + (120 * i) + 40, 0, 40, StringManager.GetString("playButton"), Color.DarkGreen, Color.Green, true)));
        }

        stopwatch.Stop();
        Log.Logger.Information($"Found levels in {stopwatch.ElapsedMilliseconds} ms");
    }

    public void Draw(SpriteBatch sprBatch)
    {
        GraphicsDevice graphDev = sprBatch.GraphicsDevice;

        for (int i = 0; i < _levels.Count; i++)
        {
            // Draw the background
            Rectangle backgroundRect = new(5, 105 + (120 * i) + (int)_scrollOffset, (int)((float)sprBatch.GraphicsDevice.Viewport.Width * 0.9f), 95);
            RectRenderer.DrawRect(backgroundRect, Color.DarkGreen, sprBatch);
            RectRenderer.DrawRectOutline(backgroundRect, Color.DarkGreen.Darker(0.5f), 3, sprBatch);

            // Draw the level name
            string label = _levels[i].Item1.ToString();
            TextRenderer.DrawText(sprBatch, "DefaultFont", 15, 110 + (120 * i) + (int)_scrollOffset, 0.56f, label, Color.White);

            _levels[i].Item2.Draw(sprBatch); // Draw the level's play button
        }

        drawHeading(graphDev, sprBatch);
        _backButton.Draw(sprBatch);
    }

    public void Update(MouseState mouse, MouseState mouseOld, KeyboardState kb, KeyboardState kbOld, ref GameState newState, ref string levelName)
    {
        updateScroll(mouse, mouseOld);

        // lerp scroll offset
        _scrollOffset = MathHelper.Lerp(_scrollOffset, _scrollOffsetGoal, 0.12f);

        for (int i = 0; i < _levels.Count; i++)
        {
            var level = _levels[i];
            level.Item2.Update(mouse, mouseOld, kb, kbOld);
            if (level.Item2.IsClicked)
            {
                Log.Logger.Information($"Clicked on level {level.Item1}");
                newState = GameState.Game;
                levelName = level.Item1.name;
            }
            _levels[i].Item2.y = 110 + (120 * i) + 40 + (int)_scrollOffset;
        }

        _backButton.Update(mouse, mouseOld, kb, kbOld);
        if (_backButton.IsClicked)
            newState = GameState.MainMenu;
    }

    private void updateScroll(MouseState mouse, MouseState mouseOld)
    {
        if (mouse.ScrollWheelValue > mouseOld.ScrollWheelValue)
            _scrollOffsetGoal += 25;
        else if (mouse.ScrollWheelValue < mouseOld.ScrollWheelValue)
            _scrollOffsetGoal -= 25;
        if (_scrollOffsetGoal > 0)
            _scrollOffsetGoal = 0;
        if (_scrollOffsetGoal < -((120 * _levels.Count) - 230))
            _scrollOffsetGoal = -((120 * _levels.Count) - 230);
    }

    private void drawHeading(GraphicsDevice graphDev, SpriteBatch sprBatch)
    {
        Rectangle nameRect = new(0, 15, graphDev.Viewport.Bounds.Width, 100);
        Rectangle nameBackgroundRect = new(0, 0, graphDev.Viewport.Bounds.Width, 100);
        RectRenderer.DrawRect(nameBackgroundRect, Color.Black, sprBatch);
        TextRenderer.DrawTextCenter(sprBatch, "DefaultFont", 0, 0, 0.9f, StringManager.GetString("selectLevel"), Color.White, nameRect);
    }
}
