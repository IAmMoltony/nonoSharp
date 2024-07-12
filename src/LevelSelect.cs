using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NonoSharp.UI;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;

namespace NonoSharp;

public class LevelSelect : IGameState
{
    public const int ScrollSpeed = 50;
    public const float KeyboardScrollSpeedMultiplier = 0.3f;
    public const int DeleteDialogWidth = 350;
    public const int DeleteDialogHeight = 270;

    private List<Tuple<LevelMetadata, LevelButtons>> _levels;
    private int _scrollOffsetGoal;
    private float _scrollOffset;
    private readonly Button _backButton;
    private string _deleteLevelName;
    private bool _deleteLevel;
    private readonly Button _deleteYesButton;
    private readonly Button _deleteNoButton;
    private Rectangle _dialogRect;

    public LevelSelect()
    {
        _scrollOffsetGoal = 0;
        _scrollOffset = 0;
        _backButton = new(10, 10, 0, 40, StringManager.GetString("back"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), Keys.Escape, true);
        _deleteLevelName = "";
        _deleteLevel = false;
        _dialogRect = new();
        _deleteNoButton = new(0, 0, 0, 40, StringManager.GetString("no"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), true);
        _deleteYesButton = new(0, 0, 0, 40, StringManager.GetString("yes"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), true);
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
            LevelMetadata metadata = list[i];

            int buttonY = 110 + (120 * i) + 40;
            Button playButton = new(15, buttonY, 0, 40, StringManager.GetString("playButton"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), true);
            Button deleteButton = metadata.isCustomLevel ? new(15, buttonY, 0, 40, StringManager.GetString("deleteButton"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), true) : null;
            LevelButtons buttons = new()
            {
                playButton = playButton,
                deleteButton = deleteButton
            };
            
            _levels.Add(new(metadata, buttons));
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

        if (_deleteLevel)
        {
            RectRenderer.DrawRect(new(0, 0, graphDev.Viewport.Bounds.Width, graphDev.Viewport.Bounds.Height), new(Color.Black, 0.5f), sprBatch);

            Rectangle dialogTextRect = _dialogRect;
            dialogTextRect.Y -= 100;
            RectRenderer.DrawRect(_dialogRect, Settings.GetDarkAccentColor(), sprBatch);
            RectRenderer.DrawRectOutline(_dialogRect, Settings.GetAccentColor(), 2, sprBatch);
            TextRenderer.DrawTextCenter(sprBatch, "DefaultFont", 0.8f, StringManager.GetString("deleteSure"), Color.White, dialogTextRect);

            TextRenderer.DrawTextWrapped(sprBatch, "DefaultFont", _dialogRect.X + 10, _dialogRect.Y + 60, 0.5f, string.Format(StringManager.GetString("deleteDialogText"), _deleteLevelName), _dialogRect.Width, Color.White);

            _deleteNoButton.Draw(sprBatch);
            _deleteYesButton.Draw(sprBatch);
        }
    }

    public IGameState? Update(MouseState mouse, MouseState mouseOld, KeyboardState kb, KeyboardState kbOld, GraphicsDevice graphDev, ref LevelMetadata levelMetadata, bool hasFocus)
    {
        if (_deleteLevel)
        {
            _dialogRect = new(graphDev.Viewport.Bounds.Width / 2 - DeleteDialogWidth / 2, graphDev.Viewport.Bounds.Height / 2 - DeleteDialogHeight / 2, DeleteDialogWidth, DeleteDialogHeight);
            _deleteNoButton.x = _dialogRect.X + 10;
            _deleteNoButton.y = _dialogRect.Y + _dialogRect.Height - 10 - _deleteNoButton.height;
            _deleteYesButton.x = _dialogRect.X + _dialogRect.Width - 10 - _deleteYesButton.width;
            _deleteYesButton.y = _deleteNoButton.y;

            _deleteNoButton.Update(mouse, mouseOld, kb, kbOld);
            _deleteYesButton.Update(mouse, mouseOld, kb, kbOld);

            if (_deleteNoButton.IsClicked)
                _deleteLevel = false;
            if (_deleteYesButton.IsClicked)
                doDelete();
        }
        else
        {
            updateScroll(mouse, mouseOld, kb, kbOld, graphDev);

            // lerp scroll offset
            _scrollOffset = MathHelper.Lerp(_scrollOffset, _scrollOffsetGoal, 0.3f);

            for (int i = 0; i < _levels.Count; i++)
            {
                var level = _levels[i];
                level.Item2.Update(mouse, mouseOld, kb, kbOld);
                if (level.Item2.playButton.IsClicked)
                {
                    Log.Logger.Information($"Clicked on level {level.Item1}");
                    levelMetadata = level.Item1;
                    Mouse.SetPosition(graphDev.Viewport.Bounds.Width / 2, graphDev.Viewport.Bounds.Height / 2); // put mouse in middle of screen
                    var play = new PlayState();
                    play.Load(levelMetadata.GetPath());
                    return play;
                }
                _levels[i].Item2.SetY(110 + (120 * i) + 40 + (int)_scrollOffset);
                if (_levels[i].Item2.deleteButton != null)
                {
                    _levels[i].Item2.deleteButton.x = _levels[i].Item2.playButton.x + _levels[i].Item2.playButton.width + 10;

                    if (_levels[i].Item2.deleteButton.IsClicked)
                    {
                        _deleteLevelName = _levels[i].Item1.name;
                        _deleteLevel = true;
                    }
                }
            }

            _backButton.Update(mouse, mouseOld, kb, kbOld);
            if (_backButton.IsClicked)
            {
                return new MainMenu();
            }
        }

        return null;
    }

    private void updateScroll(MouseState mouse, MouseState mouseOld, KeyboardState keyboard, KeyboardState keyboardOld, GraphicsDevice graphDev)
    {
        // PageUp and PageDown scroll by the window height
        if (keyboard.IsKeyDown(Keys.PageUp) && !keyboardOld.IsKeyDown(Keys.PageUp))
            _scrollOffsetGoal += graphDev.Viewport.Bounds.Height;
        else if (keyboard.IsKeyDown(Keys.PageDown) && !keyboardOld.IsKeyDown(Keys.PageDown))
            _scrollOffsetGoal -= graphDev.Viewport.Bounds.Height;

        // Home and End to go to the start and end of the list
        if (keyboard.IsKeyDown(Keys.Home) && !keyboardOld.IsKeyDown(Keys.Home))
            _scrollOffsetGoal = 0;
        else if (keyboard.IsKeyDown(Keys.End) && !keyboardOld.IsKeyDown(Keys.End))
            _scrollOffsetGoal = maxScrollOffset();

        // clamping scroll offest
        if (_scrollOffsetGoal > 0)
            _scrollOffsetGoal = 0;
        if (_scrollOffsetGoal < maxScrollOffset())
            _scrollOffsetGoal = maxScrollOffset();

        // Scrolling using mouse
        if (mouse.ScrollWheelValue > mouseOld.ScrollWheelValue)
            _scrollOffsetGoal += ScrollSpeed;
        else if (mouse.ScrollWheelValue < mouseOld.ScrollWheelValue)
            _scrollOffsetGoal -= ScrollSpeed;

        // Scrolling using arrow keys

        int keyboardScrollSpeed = (int)(ScrollSpeed * KeyboardScrollSpeedMultiplier);
        if (keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift))
            // When shift is pressed make it go faster
            keyboardScrollSpeed *= 2;

        if (keyboard.IsKeyDown(Keys.Up))
            _scrollOffsetGoal += keyboardScrollSpeed;
        else if (keyboard.IsKeyDown(Keys.Down))
            _scrollOffsetGoal -= keyboardScrollSpeed;
    }

    private int maxScrollOffset()
    {
        return -((120 * _levels.Count) - 230);
    }

    private void doDelete()
    {
        _deleteLevel = false;
        File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "Levels", $"{_deleteLevelName}.nono"));
        _levels.Remove(_levels.Find(x => x.Item1.name == _deleteLevelName));
    }

    private static void drawHeading(GraphicsDevice graphDev, SpriteBatch sprBatch)
    {
        Rectangle nameRect = new(0, 15, graphDev.Viewport.Bounds.Width, 100);
        Rectangle nameBackgroundRect = new(0, 0, graphDev.Viewport.Bounds.Width, 100);
        RectRenderer.DrawRect(nameBackgroundRect, new Color(Color.Black, 0.7f), sprBatch);
        TextRenderer.DrawTextCenter(sprBatch, "DefaultFont", 0.9f, StringManager.GetString("selectLevel"), Color.White, nameRect);
    }
}
