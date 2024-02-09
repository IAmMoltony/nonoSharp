using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace NonoSharp;

public enum GameState
{
    Game,
    MainMenu
}

public class NonoSharpGame : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Board _board;
    private MouseState _mouse;
    private MouseState _mouseOld;
    private FPSCounter _fpsCounter;
    private GameState _state;
    private int _mainMenuSelect;

    private static string[] _mainMenuButtons = {
        "Play",
        "Quit"
    };

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

    public NonoSharpGame()
    {
        _state = GameState.MainMenu;
        _mainMenuSelect = -1;
        _fpsCounter = new();
        _solveTime = 0;
        _board = new ("Levels/TestLevel.nono");
        _graphics = new(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _solveTimeThread = new(SolveTimeTick);
    }

    protected override void Initialize()
    {
        _graphics.IsFullScreen = false;
        _graphics.PreferredBackBufferWidth = 800;
        _graphics.PreferredBackBufferHeight = 600;
        _graphics.ApplyChanges();
        Window.AllowUserResizing = true;

        _solveTimeTick = false;
        _solveTimeThread.Start();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        RectRenderer.Load(GraphicsDevice);
        GridRenderer.Load(GraphicsDevice);
        Tile.LoadTextures(Content);
        TextRenderer.LoadFont("notosans", "notosans", Content);
    }

    protected override void Update(GameTime gameTime)
    {
        _mouseOld = _mouse;
        _mouse = Mouse.GetState();

        switch (_state)
        {
        case GameState.Game:
            _board.Update(_mouse, _mouseOld, GraphicsDevice);
            if (_board.IsSolved)
                _solveTimeTick = false;
            break;
        case GameState.MainMenu:
            if (_mouse.X >= 10 && _mouse.Y >= 100 && _mouse.X <= 70 && _mouse.Y <= 129)
                _mainMenuSelect = 0;
            else if (_mouse.X >= 10 && _mouse.Y >= 124 && _mouse.X <= 70 && _mouse.Y <= 158)
                _mainMenuSelect = 1;
            else
                _mainMenuSelect = -1;

            if (_mouseOld.LeftButton == ButtonState.Released && _mouse.LeftButton == ButtonState.Pressed)
            {
                switch (_mainMenuSelect)
                {
                case 0:
                    _solveTimeTick = true;
                    _state = GameState.Game;
                    break;
                case 1:
                    Exit();
                    break;
                }
            }
            break;
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();

        switch (_state)
        {
        case GameState.Game:
            _board.Draw(_spriteBatch, GraphicsDevice);
            TextRenderer.DrawText(_spriteBatch, "notosans", 10, 10, 0.6f, $"Time: {Math.Round(_solveTime / 1000, 2)} s", _board.IsSolved ? Color.Lime : Color.White);
            break;
        case GameState.MainMenu:
            TextRenderer.DrawText(_spriteBatch, "notosans", 10, 10, "nonoSharp", Color.White);
            drawMainMenuButtons();
            break;
        }

        TextRenderer.DrawText(_spriteBatch, "notosans", 10, GraphicsDevice.Viewport.Bounds.Height - 26, 0.33f, $"{Math.Round(_fpsCounter.CurrentFPS)} fps, {Math.Round(_fpsCounter.AverageFPS)} avg", Color.LightGray);

        _spriteBatch.End();

        _fpsCounter.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

        base.Draw(gameTime);
    }

    private void drawMainMenuButtons()
    {
        for (int i = 0; i < _mainMenuButtons.Length; i++)
        {
            bool selected = _mainMenuSelect == i;
            string buttonText = selected ? $"> {_mainMenuButtons[i]} <" : _mainMenuButtons[i];
            Color color = selected ? Color.Lime : Color.White;
            TextRenderer.DrawText(_spriteBatch, "notosans", 10, 100 + 29 * i, 0.5f, buttonText, color);
        }
    }

    protected override void OnExiting(object sender, EventArgs e)
    {
        _solveTimeThreadRunning = false;
        _solveTimeThread.Join();
        base.OnExiting(sender, e);
    }
}
