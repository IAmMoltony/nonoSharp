using System;
using System.Threading;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Serilog;

namespace NonoSharp;

public enum GameState
{
    None, // Used to indicate the absense of a game state
    Game,
    MainMenu,
    LevelSelect,
    Editor
}

public class NonoSharpGame : Game
{
    public static MouseCursor Cursor;

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
    private LevelSelect _levelSelect;
    private Editor.Editor _editor;

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
        CrashHandler.Initialize();

        // initialize serilog
        using var log = new LoggerConfiguration().WriteTo.Console().WriteTo.File($"{AppDomain.CurrentDomain.BaseDirectory}/logs/nonoSharp.log", rollingInterval: RollingInterval.Day).CreateLogger();
        Log.Logger = log;

        _state = GameState.MainMenu;
        _fpsCounter = new();
        _solveTime = 0;
        _board = new();
        _graphics = new(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _solveTimeThread = new(SolveTimeTick);
        _mainMenu = new();
        _levelSelect = new();
        _editor = new();
    }

    protected override void Initialize()
    {
        Window.Title = $"nonoSharp {GameVersion.GetGameVersion()}";

        _graphics.IsFullScreen = false;
        _graphics.PreferredBackBufferWidth = 800;
        _graphics.PreferredBackBufferHeight = 600;
        _graphics.ApplyChanges();
        Window.AllowUserResizing = true;

        Window.TextInput += doTextInput;

        _solveTimeTick = false;
        _solveTimeThread.Start();

        Log.Logger.Information("nonoSharp initialized");

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new(GraphicsDevice);

        Stopwatch stopwatch = new();
        stopwatch.Start();

        RectRenderer.Load(GraphicsDevice);
        GridRenderer.Load(GraphicsDevice);
        Tile.LoadTextures(Content);
        TextRenderer.LoadFont("notosans", "notosans", Content);

        stopwatch.Stop();

        Log.Logger.Information($"Loaded content in {stopwatch.ElapsedMilliseconds} ms");
    }

    protected override void Update(GameTime gameTime)
    {
        _mouseOld = _mouse;
        _mouse = Mouse.GetState();
        _kb = Keyboard.GetState();
        _kbOld = _kb;

        Cursor = MouseCursor.Arrow;

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
                    _levelSelect.FindLevels();
                    _state = GameState.LevelSelect;
                }
                else if (_mainMenu.QuitButton.IsClicked)
                    Exit();
                else if (_mainMenu.EditorButton.IsClicked)
                    _state = GameState.Editor;
                break;
            case GameState.LevelSelect:
                GameState newState = GameState.None;
                string levelName = "";
                _levelSelect.Update(_mouse, _mouseOld, _kb, _kbOld, ref newState, ref levelName);
                if (newState == GameState.Game)
                {
                    _solveTimeTick = true;
                    _board.Load($"{AppDomain.CurrentDomain.BaseDirectory}/Content/Levels/{levelName}.nono");
                }
                if (newState != GameState.None)
                    _state = newState;
                break;
            case GameState.Editor:
                _editor.Update(_mouse, _mouseOld, _kb, _kbOld, GraphicsDevice);
                break;
        }

        Mouse.SetCursor(Cursor);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();

        switch (_state)
        {
            case GameState.Game:
                _board.Draw(_spriteBatch);
                TextRenderer.DrawText(_spriteBatch, "notosans", 10, 10, 0.6f, $"Time: {Math.Round(_solveTime / 1000, 2)} s", _board.IsSolved ? Color.Lime : Color.White);
                break;
            case GameState.MainMenu:
                _mainMenu.Draw(_spriteBatch);
                break;
            case GameState.LevelSelect:
                _levelSelect.Draw(_spriteBatch);
                break;
            case GameState.Editor:
                _editor.Draw(_spriteBatch);
                break;
        }

        TextRenderer.DrawText(_spriteBatch, "notosans", 10, GraphicsDevice.Viewport.Bounds.Height - 26, 0.33f, $"{Math.Round(_fpsCounter.CurrentFPS)} fps, {Math.Round(_fpsCounter.AverageFPS)} avg", Color.LightGray);

        _spriteBatch.End();

        _fpsCounter.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

        base.Draw(gameTime);
    }

    protected override void OnExiting(object sender, EventArgs e)
    {
        Log.Logger.Information("Stopping solve time thread");
        _solveTimeThreadRunning = false;
        _solveTimeThread.Join();

        base.OnExiting(sender, e);
    }

    private void doTextInput(object sender, TextInputEventArgs tiea)
    {
        if (_state == GameState.Editor)
            _editor.UpdateInput(sender, tiea);
    }
}
