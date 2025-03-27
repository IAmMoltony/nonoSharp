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
    public const int DialogWidth = 350;
    public const int DialogHeight = 270;

    private List<Tuple<LevelMetadata, LevelButtons>> _levels;
    private int _scrollOffsetGoal;
    private float _scrollOffset;
    private readonly Button _backButton;
    private string _modifyLevelName;
    private bool _deleteLevel;
    private bool _renameLevel;
    private readonly Button _deleteNoButton;
    private readonly Button _deleteYesButton;
    private readonly Button _renameCancelButton;
    private readonly Button _renameOKButton;
    private readonly TextBox _renameBox;
    private Dialog _renameDeleteDialog;

    public LevelSelect()
    {
        _scrollOffsetGoal = 0;
        _scrollOffset = 0;
        _backButton = new(10, 10, 0, 40, StringManager.GetString("back"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), Keys.Escape, true);
        _modifyLevelName = "";
        _deleteLevel = false;
        _deleteNoButton = new(0, 0, 0, 40, StringManager.GetString("no"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), true, 16);
        _deleteYesButton = new(0, 0, 0, 40, StringManager.GetString("yes"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), true, 16);
        _renameCancelButton = new(0, 0, 0, 40, StringManager.GetString("cancel"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), true);
        _renameOKButton = new(0, 0, 0, 40, StringManager.GetString("ok"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), true);
        _levels = new();

        _renameBox = new(0, 0, 0, Color.DarkGray, Color.Gray, Color.White, Color.White, 230);
        char[] invalidPathChars = Path.GetInvalidPathChars();
        char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
        _renameBox.illegalChars = invalidPathChars.Union(invalidFileNameChars).ToList();

        _renameDeleteDialog = new(DialogWidth, DialogHeight, "", Settings.GetDarkAccentColor(), Settings.GetAccentColor());

        FindLevels();
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
            Button? deleteButton = metadata.isCustomLevel ? new(15, buttonY, 0, 40, StringManager.GetString("deleteButton"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), true) : null;
            Button? renameButton = metadata.isCustomLevel ? new(15, buttonY, 0, 40, StringManager.GetString("renameButton"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), true) : null;
            Button? editButton = metadata.isCustomLevel ? new(15, buttonY, 0, 40, StringManager.GetString("editButton"), Settings.GetDarkAccentColor(), Settings.GetAccentColor(), true) : null;
            LevelButtons buttons = new()
            {
                playButton = playButton,
                deleteButton = deleteButton,
                renameButton = renameButton,
                editButton = editButton
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
            _renameDeleteDialog.SetTitle(StringManager.GetString("deleteSure"));
            _renameDeleteDialog.Draw(sprBatch);
            TextRenderer.DrawTextWrapped(sprBatch, "DefaultFont", _renameDeleteDialog.rect.X + 10, _renameDeleteDialog.rect.Y + 60, 0.5f, string.Format(StringManager.GetString("deleteDialogText"), _modifyLevelName), _renameDeleteDialog.rect.Width, Color.White);

            _deleteNoButton.Draw(sprBatch);
            _deleteYesButton.Draw(sprBatch);
        }
        else if (_renameLevel)
        {
            _renameDeleteDialog.SetTitle(StringManager.GetString("renameLevel"));
            _renameDeleteDialog.Draw(sprBatch);
            _renameBox.Draw(sprBatch);
            _renameCancelButton.Draw(sprBatch);
            _renameOKButton.Draw(sprBatch);
        }
    }

    public IGameState? Update(MouseState mouse, MouseState mouseOld, KeyboardState kb, KeyboardState kbOld, GraphicsDevice graphDev, ref LevelMetadata levelMetadata, bool hasFocus)
    {
        if (_deleteLevel || _renameLevel)
        {
            _renameDeleteDialog.Update(graphDev);
        }

        if (_deleteLevel)
        {
            _renameDeleteDialog.rect.Width = Math.Max(DialogWidth, (int)TextRenderer.MeasureString("DefaultFont", StringManager.GetString("deleteSure")).X + 12);

            _deleteNoButton.x = _renameDeleteDialog.rect.X + 10;
            _deleteNoButton.y = _renameDeleteDialog.rect.Y + _renameDeleteDialog.rect.Height - 10 - _deleteNoButton.height;
            _deleteYesButton.x = _renameDeleteDialog.rect.X + _renameDeleteDialog.rect.Width - 10 - _deleteYesButton.width;
            _deleteYesButton.y = _deleteNoButton.y;

            _deleteNoButton.Update(mouse, mouseOld, kb, kbOld);
            _deleteYesButton.Update(mouse, mouseOld, kb, kbOld);

            if (_deleteNoButton.IsClicked)
                _deleteLevel = false;
            else if (_deleteYesButton.IsClicked)
                doDelete();
        }
        else if (_renameLevel)
        {
            _renameDeleteDialog.rect.Width = Math.Max(DialogWidth, (int)TextRenderer.MeasureString("DefaultFont", StringManager.GetString("renameLevel")).X + 12);

            _renameBox.x = _renameDeleteDialog.rect.X + 10;
            _renameBox.width = _renameDeleteDialog.rect.Width - 30;
            _renameBox.y = _renameDeleteDialog.rect.Y + 32 + TextBox.Height;

            _renameCancelButton.x = _renameDeleteDialog.rect.X + 10;
            _renameCancelButton.y = _renameDeleteDialog.rect.Y + _renameDeleteDialog.rect.Height - 10 - _renameCancelButton.height;
            _renameOKButton.x = _renameDeleteDialog.rect.X + _renameDeleteDialog.rect.Width - 10 - _renameOKButton.width;
            _renameOKButton.y = _renameCancelButton.y;

            _renameBox.Update(mouse, mouseOld, kb, kbOld);
            _renameCancelButton.Update(mouse, mouseOld, kb, kbOld);
            _renameOKButton.Update(mouse, mouseOld, kb, kbOld);

            if (_renameCancelButton.IsClicked)
            {
                _renameLevel = false;
                _renameBox.Clear();
            }
            else if (_renameOKButton.IsClicked)
                doRename();
        }
        else
        {
            _backButton.Update(mouse, mouseOld, kb, kbOld);
            if (_backButton.IsClicked)
            {
                return new MainMenu();
            }

            updateScroll(mouse, mouseOld, kb, kbOld, graphDev);

            // lerp scroll offset
            _scrollOffset = MathHelper.Lerp(_scrollOffset, _scrollOffsetGoal, 0.3f);

            for (int i = 0; i < _levels.Count; i++)
            {
                var level = _levels[i];
                level.Item2.Update(mouse, mouseOld, kb, kbOld);
                if (level.Item2.playButton?.IsClicked == true)
                {
                    Log.Logger.Information($"Clicked on level {level.Item1}");
                    levelMetadata = level.Item1;
                    Mouse.SetPosition(graphDev.Viewport.Bounds.Width / 2, graphDev.Viewport.Bounds.Height / 2); // put mouse in middle of screen
                    var play = new PlayState();
                    play.Load(levelMetadata.GetPath());
                    return play;
                }
                _levels[i].Item2.SetY(110 + (120 * i) + 40 + (int)_scrollOffset);

                if (level.Item2.deleteButton != null)
                {
                    if (level.Item2.playButton != null)
                        level.Item2.deleteButton.x = level.Item2.playButton.x + level.Item2.playButton.width + 10;

                    if (level.Item2.deleteButton.IsClicked)
                    {
                        _modifyLevelName = _levels[i].Item1.name;
                        _deleteLevel = true;
                    }
                }

                if (level.Item2.renameButton != null)
                {
                    if (level.Item2.deleteButton != null)
                        level.Item2.renameButton.x = level.Item2.deleteButton.x + level.Item2.deleteButton.width + 10;
                    else if (level.Item2.playButton != null)
                        level.Item2.renameButton.x = level.Item2.playButton.x + level.Item2.playButton.width + 10;

                    if (level.Item2.renameButton.IsClicked)
                    {
                        _modifyLevelName = _levels[i].Item1.name;
                        _renameLevel = true;
                        _renameBox.text = _modifyLevelName;
                        _renameOKButton.disabled = true;
                    }
                }

                if (level.Item2.editButton != null)
                {
                    if (level.Item2.renameButton != null)
                        level.Item2.editButton.x = level.Item2.renameButton.x + level.Item2.renameButton.width + 10;
                    else if (level.Item2.deleteButton != null)
                        level.Item2.editButton.x = level.Item2.deleteButton.x + level.Item2.deleteButton.width + 10;
                    else if (level.Item2.playButton != null)
                        level.Item2.editButton.x = level.Item2.playButton.x + level.Item2.playButton.width + 10;

                    if (level.Item2.editButton.IsClicked)
                    {
                        return new Editor.Editor(level.Item1.name);
                    }
                }
            }
        }

        return null;
    }

    public void UpdateInput(TextInputEventArgs tiea)
    {
        if (_renameLevel)
        {
            _renameBox.UpdateInput(tiea);

            // disable if new name either same as old name or empty
            _renameOKButton.disabled = _renameBox.text == _modifyLevelName || string.IsNullOrWhiteSpace(_renameBox.text);
        }
    }

    private void updateScroll(MouseState mouse, MouseState mouseOld, KeyboardState keyboard, KeyboardState keyboardOld, GraphicsDevice graphDev)
    {
        updateScrollKeyboard(keyboard, keyboardOld, graphDev);
        clampScrollOffset();
        updateScrollMouse(mouse, mouseOld);
        updateScrollArrows(keyboard, keyboardOld);
    }

    private void updateScrollKeyboard(KeyboardState keyboard, KeyboardState keyboardOld, GraphicsDevice graphDev)
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
    }

    private void clampScrollOffset()
    {
        if (_scrollOffsetGoal > 0)
            _scrollOffsetGoal = 0;
        if (_scrollOffsetGoal < maxScrollOffset())
            _scrollOffsetGoal = maxScrollOffset();
    }

    private void updateScrollMouse(MouseState mouse, MouseState mouseOld)
    {
        if (mouse.ScrollWheelValue > mouseOld.ScrollWheelValue)
            _scrollOffsetGoal += ScrollSpeed;
        else if (mouse.ScrollWheelValue < mouseOld.ScrollWheelValue)
            _scrollOffsetGoal -= ScrollSpeed;
    }

    private void updateScrollArrows(KeyboardState keyboard, KeyboardState keyboardOld)
    {
        int keyboardScrollSpeed = (int)(ScrollSpeed * KeyboardScrollSpeedMultiplier);
        if (keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift))
            // When shift is pressed make it go faster
            keyboardScrollSpeed *= 2;

        if (keyboard.IsKeyDown(Keys.Up))
            _scrollOffsetGoal += keyboardScrollSpeed;
        else if (keyboard.IsKeyDown(Keys.Down))
            _scrollOffsetGoal -= keyboardScrollSpeed;
    }

    private void doDelete()
    {
        _deleteLevel = false;
        File.Delete(Path.Combine(BoardSaver.GetLevelSavePath(), $"{_modifyLevelName}.nono"));
        var level = _levels.Find(x => x.Item1.name == _modifyLevelName);
        FindLevels();
    }

    private void doRename()
    {
        _renameLevel = false;
        string trimmedNewName = _renameBox.text.Trim();
        File.Move(Path.Combine(BoardSaver.GetLevelSavePath(), $"{_modifyLevelName}.nono"), Path.Combine(BoardSaver.GetLevelSavePath(), $"{trimmedNewName}.nono"));
        FindLevels();
    }

    private static void drawHeading(GraphicsDevice graphDev, SpriteBatch sprBatch)
    {
        Rectangle nameRect = new(0, 15, graphDev.Viewport.Bounds.Width, 100);
        Rectangle nameBackgroundRect = new(0, 0, graphDev.Viewport.Bounds.Width, 100);
        RectRenderer.DrawRect(nameBackgroundRect, new Color(Color.Black, 0.7f), sprBatch);
        TextRenderer.DrawTextCenter(sprBatch, "DefaultFont", 0.9f, StringManager.GetString("selectLevel"), Color.White, nameRect);
    }

    private int maxScrollOffset() => -((120 * _levels.Count) - 230);
}
