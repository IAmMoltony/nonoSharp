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

    private MouseState _mouse;
    private MouseState _mouseOld;

    private KeyboardState _kb;
    private KeyboardState _kbOld;

    private FPSCounter _fpsCounter;

    private GameState _state;
    private MainMenu _mainMenu;
    private LevelSelect _levelSelect;
    private PlayState _play;
    private Editor.Editor _editor;

    public NonoSharpGame()
    {
        CrashHandler.Initialize();

        // initialize serilog
        using var log = new LoggerConfiguration().WriteTo.Console().WriteTo.File($"{AppDomain.CurrentDomain.BaseDirectory}/logs/nonoSharp.log", rollingInterval: RollingInterval.Day).CreateLogger();
        Log.Logger = log; // global logger

        _state = GameState.MainMenu;
        _fpsCounter = new();
        _graphics = new(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _mainMenu = new();
        _levelSelect = new();
        _editor = new();
        _play = new();
    }

    protected override void Initialize()
    {
        Window.Title = $"nonoSharp {GameVersion.GetGameVersion()}";

        _graphics.IsFullScreen = false; // disable fullscreen
        
        // initial window size: 85% of monitor size
        int monitorWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        int monitorHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        _graphics.PreferredBackBufferWidth = (int)(monitorWidth * 0.85f);
        _graphics.PreferredBackBufferHeight = (int)(monitorHeight * 0.85f);
        _graphics.ApplyChanges();

        // allow the window to be resized
        Window.AllowUserResizing = true;

        Window.TextInput += doTextInput;

        Log.Logger.Information("nonoSharp initialized");

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new(GraphicsDevice);

        Stopwatch stopwatch = new();
        stopwatch.Start();

        // load stuff renderers
        RectRenderer.Load(GraphicsDevice);
        GridRenderer.Load(GraphicsDevice);

        Tile.LoadTextures(Content); // load tile textures
        TextRenderer.LoadFont("notosans", "notosans", Content); // load noto sans font (i think it's a nice font)

        stopwatch.Stop();

        Log.Logger.Information($"Loaded content in {stopwatch.ElapsedMilliseconds} ms");
    }

    protected override void Update(GameTime gameTime)
    {
        // get input
        _mouseOld = _mouse;
        _mouse = Mouse.GetState();
        _kb = Keyboard.GetState();
        _kbOld = _kb;

        Cursor = MouseCursor.Arrow;

        switch (_state)
        {
            case GameState.Game:
                _play.Update(_mouse, _mouseOld, GraphicsDevice);
                break;
            case GameState.MainMenu:
                _mainMenu.Update(_mouse, _mouseOld, _kb, _kbOld, GraphicsDevice);

                // when play button is pressed then go to level select
                if (_mainMenu.PlayButton.IsClicked)
                {
                    _levelSelect.FindLevels();
                    _state = GameState.LevelSelect;
                }
                // quit button
                else if (_mainMenu.QuitButton.IsClicked)
                    Exit();
                // editor button
                else if (_mainMenu.EditorButton.IsClicked)
                    _state = GameState.Editor;
                break;
            case GameState.LevelSelect:
            {
                GameState newState = GameState.None;
                string levelName = "";
                _levelSelect.Update(_mouse, _mouseOld, _kb, _kbOld, ref newState, ref levelName);
                if (newState == GameState.Game)
                    _play.Load($"{AppDomain.CurrentDomain.BaseDirectory}/Content/Levels/{levelName}.nono");
                if (newState != GameState.None)
                    _state = newState;
                break;
            }
            case GameState.Editor:
            {
                GameState newState;
                _editor.Update(_mouse, _mouseOld, _kb, _kbOld, GraphicsDevice, out newState);
                if (newState != GameState.None)
                    _state = newState;
                break;
            }
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
                _play.Draw(_spriteBatch);
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

        // draw fps
        TextRenderer.DrawText(_spriteBatch, "notosans", 10, GraphicsDevice.Viewport.Bounds.Height - 26, 0.33f, $"{Math.Round(_fpsCounter.CurrentFPS)} fps, {Math.Round(_fpsCounter.AverageFPS)} avg", Color.LightGray);

        _spriteBatch.End();

        _fpsCounter.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

        base.Draw(gameTime);
    }

    protected override void OnExiting(object sender, EventArgs e)
    {
        _play.StopSolveTimeThread();

        base.OnExiting(sender, e);
    }

    private void doTextInput(object sender, TextInputEventArgs tiea)
    {
        // update editor input when in editor
        if (_state == GameState.Editor)
            _editor.UpdateInput(sender, tiea);
    }
}
