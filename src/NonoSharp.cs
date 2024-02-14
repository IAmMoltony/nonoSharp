using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NonoSharp.UI;

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
    private KeyboardState _kb;
    private KeyboardState _kbOld;
    private FPSCounter _fpsCounter;
    private GameState _state;
    private MainMenu _mainMenu;

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
        _fpsCounter = new();
        _solveTime = 0;
        _board = new ("Content/Levels/TestLevel.nono");
        _graphics = new(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _solveTimeThread = new(SolveTimeTick);
        _mainMenu = new();
    }

    protected override void Initialize()
    {
        Window.Title = $"nonoSharp {GameVersion.GetGameVersion()}";

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
        _spriteBatch = new(GraphicsDevice);

        RectRenderer.Load(GraphicsDevice);
        GridRenderer.Load(GraphicsDevice);
        Tile.LoadTextures(Content);
        TextRenderer.LoadFont("notosans", "notosans", Content);
    }

    protected override void Update(GameTime gameTime)
    {
        _mouseOld = _mouse;
        _mouse = Mouse.GetState();
        _kb = Keyboard.GetState();
        _kbOld = _kb;

        switch (_state)
        {
        case GameState.Game:
            _board.Update(_mouse, _mouseOld, GraphicsDevice);
            if (_board.IsSolved)
                _solveTimeTick = false;
            break;
        case GameState.MainMenu:
            _mainMenu.Update(_mouse, _mouseOld, _kb, _kbOld, GraphicsDevice);
            if (_mainMenu.PlayButton.IsClicked)
            {
                _solveTimeTick = true;
                _state = GameState.Game;
            }
            else if (_mainMenu.QuitButton.IsClicked)
                Exit();
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
            _mainMenu.Draw(_spriteBatch, GraphicsDevice);
            break;
        }

        TextRenderer.DrawText(_spriteBatch, "notosans", 10, GraphicsDevice.Viewport.Bounds.Height - 26, 0.33f, $"{Math.Round(_fpsCounter.CurrentFPS)} fps, {Math.Round(_fpsCounter.AverageFPS)} avg", Color.LightGray);

        _spriteBatch.End();

        _fpsCounter.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

        base.Draw(gameTime);
    }

    protected override void OnExiting(object sender, EventArgs e)
    {
        // stop solve time thread
        _solveTimeThreadRunning = false;
        _solveTimeThread.Join();

        base.OnExiting(sender, e);
    }
}
