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
    public static readonly int ScrollSpeed = 50;
    public static readonly float KeyboardScrollSpeedMultiplier = 0.3f;

    private List<Tuple<LevelMetadata, LevelButtons>> _levels;
    private int _scrollOffsetGoal;
    private float _scrollOffset;
    private readonly Button _backButton;

    public LevelSelect()
    {
        _scrollOffsetGoal = 0;
        _scrollOffset = 0;
        _backButton = new(10, 10, 0, 40, StringManager.GetString("back"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), Keys.Escape, true);
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
            int buttonY = 110 + (120 * i) + 40;
            Button playButton = new(15, buttonY, 0, 40, StringManager.GetString("playButton"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), true);
            Button deleteButton = new(15, buttonY, 0, 40, StringManager.GetString("deleteButton"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), true);
            LevelButtons buttons = new()
            {
                playButton = playButton,
                deleteButton = deleteButton
            };
            
            _levels.Add(new(list[i], buttons));
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
            Rectangle backgroundRect = new(5, 105 + (120 * i) + (int)_scrollOffset, (int)((float)sprBatch.GraphicsDevice.Viewport.Width * 0.8f), 95);
            RectRenderer.DrawRect(backgroundRect, Settings.GetDarkAccentColor(), sprBatch);
            RectRenderer.DrawRectOutline(backgroundRect, Settings.GetDarkAccentColor(0.8f), 3, sprBatch);

            // Draw the level name
            string label = _levels[i].Item1.ToString();
            TextRenderer.DrawText(sprBatch, "DefaultFont", 15, 110 + (120 * i) + (int)_scrollOffset, 0.56f, label, Color.White);

            _levels[i].Item2.Draw(sprBatch); // Draw the level's play button
        }

        drawHeading(graphDev, sprBatch);
        _backButton.Draw(sprBatch);
    }

    public void Update(MouseState mouse, MouseState mouseOld, KeyboardState kb, KeyboardState kbOld, out GameState? newState, ref string levelName)
    {
        newState = null;

        updateScroll(mouse, mouseOld, kb);

        // lerp scroll offset
        _scrollOffset = MathHelper.Lerp(_scrollOffset, _scrollOffsetGoal, 0.3f);

        for (int i = 0; i < _levels.Count; i++)
        {
            var level = _levels[i];
            level.Item2.Update(mouse, mouseOld, kb, kbOld);
            if (level.Item2.playButton.IsClicked)
            {
                Log.Logger.Information($"Clicked on level {level.Item1}");
                newState = GameState.Game;
                levelName = level.Item1.name;
            }
            _levels[i].Item2.SetY(110 + (120 * i) + 40 + (int)_scrollOffset);
            _levels[i].Item2.deleteButton.x = _levels[i].Item2.playButton.x + _levels[i].Item2.playButton.width + 10;
        }

        _backButton.Update(mouse, mouseOld, kb, kbOld);
        if (_backButton.IsClicked)
            newState = GameState.MainMenu;
    }

    private void updateScroll(MouseState mouse, MouseState mouseOld, KeyboardState keyboard)
    {
        if (_scrollOffsetGoal > 0)
            _scrollOffsetGoal = 0;
        if (_scrollOffsetGoal < -((120 * _levels.Count) - 230))
            _scrollOffsetGoal = -((120 * _levels.Count) - 230);
        if (mouse.ScrollWheelValue > mouseOld.ScrollWheelValue)
            _scrollOffsetGoal += ScrollSpeed;
        else if (mouse.ScrollWheelValue < mouseOld.ScrollWheelValue)
            _scrollOffsetGoal -= ScrollSpeed;

        // arrow keys
        int keyboardScrollSpeed = (int)(ScrollSpeed * KeyboardScrollSpeedMultiplier);
        if (keyboard.IsKeyDown(Keys.Up))
            _scrollOffsetGoal += keyboardScrollSpeed;
        else if (keyboard.IsKeyDown(Keys.Down))
            _scrollOffsetGoal -= keyboardScrollSpeed;
    }

    private static void drawHeading(GraphicsDevice graphDev, SpriteBatch sprBatch)
    {
        Rectangle nameRect = new(0, 15, graphDev.Viewport.Bounds.Width, 100);
        Rectangle nameBackgroundRect = new(0, 0, graphDev.Viewport.Bounds.Width, 100);
        RectRenderer.DrawRect(nameBackgroundRect, new Color(Color.Black, 0.7f), sprBatch);
        TextRenderer.DrawTextCenter(sprBatch, "DefaultFont", 0.9f, StringManager.GetString("selectLevel"), Color.White, nameRect);
    }
}
