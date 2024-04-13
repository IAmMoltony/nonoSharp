using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Serilog;
using System;
using System.Diagnostics;

namespace NonoSharp;

public enum GameState
{
    None, // Used to indicate the absense of a game state (TODO: check if we can just use null)
    Game,
    MainMenu,
    LevelSelect,
    Editor
}

public class NonoSharpGame : Game
{
    public static MouseCursor Cursor;

    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Process _gameProcess;

    private MouseState _mouse;
    private MouseState _mouseOld;

    private KeyboardState _kb;
    private KeyboardState _kbOld;

    private FPSCounter _fpsCounter;
    private bool _showFPS;

    private GameState _state;
    private MainMenu _mainMenu;
    private LevelSelect _levelSelect;
    private PlayState _play;
    private Editor.Editor _editor;

    public NonoSharpGame()
    {
        CrashHandler.Initialize();

        // Initialize serilog
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(
                    outputTemplate: "{Timestamp:HH:mm:ss} {Level:u4} {Message:lj}{NewLine}"
                )
            .WriteTo.File(
                    $"{AppDomain.CurrentDomain.BaseDirectory}/logs/nonoSharp.log",
                    rollingInterval: RollingInterval.Minute,
                    retainedFileCountLimit: 15
                )
            .CreateLogger();

#if DEBUG
        // print if we're in debug config
        Log.Logger.Information("Debug configuration!");
#endif

        _graphics = new(this);
        Content.RootDirectory = "Content";
    }

    protected override void Initialize()
    {
        Stopwatch stopwatch = new();
        stopwatch.Start();

        Window.Title = $"nonoSharp {GameVersion.GetGameVersion()}";

        _gameProcess = Process.GetCurrentProcess();

        Settings.Initialize();
        StringManager.Initialize();

        _state = GameState.MainMenu;
        _fpsCounter = new();
        _showFPS = false;
        IsMouseVisible = true;
        _mainMenu = new();
        _levelSelect = new();
        _editor = new();
        _play = new();

        _graphics.HardwareModeSwitch = false;

        if (Settings.GetBool("fullScreen"))
        {
            setWindowSize(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height, true);
        }
        else
        {
            // initial window size: 85% of monitor size
            setWindowSize((int)(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width * 0.85f), (int)(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height * 0.85f), false);
        }

        // allow the window to be resized
        Window.AllowUserResizing = true;

        Window.TextInput += doTextInput;

        stopwatch.Stop();
        Log.Logger.Information($"nonoSharp initialized in {stopwatch.ElapsedMilliseconds} ms");

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
        TextRenderer.LoadFont("DefaultFont", "DefaultFont", Content); // load noto sans font (i think it's a nice font)

        stopwatch.Stop();

        Log.Logger.Information($"Loaded content in {stopwatch.ElapsedMilliseconds} ms");
    }

    protected override void Update(GameTime gameTime)
    {
        getInput();

        Cursor = MouseCursor.Arrow;

        switch (_state)
        {
            case GameState.Game:
                _play.Update(_mouse, _mouseOld, _kb, _kbOld, GraphicsDevice, out bool leave);
                if (leave)
                    _state = GameState.LevelSelect;
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
                    {
                        Mouse.SetPosition(GraphicsDevice.Viewport.Bounds.Width / 2, GraphicsDevice.Viewport.Bounds.Height / 2); // put mouse in middle of screen
                        _play.Load($"{AppDomain.CurrentDomain.BaseDirectory}/Content/Levels/{levelName}.nono");
                    }
                    if (newState != GameState.None)
                        _state = newState;
                    break;
                }
            case GameState.Editor:
                {
                    _editor.Update(_mouse, _mouseOld, _kb, _kbOld, GraphicsDevice, out GameState newState);
                    if (newState != GameState.None)
                        _state = newState;
                    break;
                }
        }

        updateShowFPS();
        updatePerfInfo();

        if (_kb.IsKeyDown(Keys.F11) && !_kbOld.IsKeyDown(Keys.F11))
            toggleFullScreen();

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

        // draw some performance info
        if (_showFPS)
        {
            TextRenderer.DrawText(_spriteBatch, "DefaultFont", 10, GraphicsDevice.Viewport.Bounds.Height - 26, 0.33f, $"{Math.Round(_fpsCounter.CurrentFPS)} fps, {Math.Round(_fpsCounter.AverageFPS)} avg", Color.LightGray); // FPS
            TextRenderer.DrawText(_spriteBatch, "DefaultFont", 10, GraphicsDevice.Viewport.Bounds.Height - 42, 0.33f, $"mem: {Math.Round(((float)_gameProcess.WorkingSet64 / 1024 / 1024), 2)}M (peak {Math.Round(((float)_gameProcess.PeakWorkingSet64 / 1024 / 1024), 2)}M)", Color.LightGray); // Memory usage (current and peak)
        }

        _spriteBatch.End();

        _fpsCounter.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

        base.Draw(gameTime);
    }

    protected override void OnExiting(object sender, EventArgs e)
    {
        PlayState.StopSolveTimeThread();
        Settings.Save();

        base.OnExiting(sender, e);
    }

    private void doTextInput(object sender, TextInputEventArgs tiea)
    {
        // update editor input when in editor
        if (_state == GameState.Editor)
            _editor.UpdateInput(sender, tiea);
    }

    private void getInput()
    {
        _mouseOld = _mouse;
        _kbOld = _kb;
        _mouse = Mouse.GetState();
        _kb = Keyboard.GetState();
    }

    private void updateShowFPS()
    {
        if (_kb.IsKeyDown(Keys.F12) && !_kbOld.IsKeyDown(Keys.F12))
            _showFPS = !_showFPS;
    }

    private void updatePerfInfo()
    {
        if (_fpsCounter.TotalFrames % 50 == 0 && _showFPS)
            _gameProcess.Refresh();
    }

    private void toggleFullScreen()
    {
        if (_graphics.IsFullScreen)
            setWindowSize((int)(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width * 0.85f), (int)(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height * 0.85f), false);
        else
            setWindowSize(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height, true);
    }

    private void setWindowSize(int w, int h, bool fullscreen)
    {
        _graphics.IsFullScreen = fullscreen;
        _graphics.PreferredBackBufferWidth = w;
        _graphics.PreferredBackBufferHeight = h;
        _graphics.ApplyChanges();
        Settings.Set("fullScreen", fullscreen);
    }
}
