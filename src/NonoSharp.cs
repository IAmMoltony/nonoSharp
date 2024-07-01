using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Serilog;
using System;
using System.IO;
using System.Diagnostics;

namespace NonoSharp;

public enum GameState
{
    Game,
    MainMenu,
    LevelSelect,
    Editor,
    Settings,
    Credits
}

public class NonoSharpGame : Game
{
    public static MouseCursor Cursor { get; set; }

    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private MouseState _mouse;
    private MouseState _mouseOld;

    private KeyboardState _kb;
    private KeyboardState _kbOld;

    private PerformanceInfo _perfInfo;
    private bool _showPerformanceInfo;

    private bool _showBgGrid;

    private GameState _state;
    private MainMenu _mainMenu;
    private LevelSelect _levelSelect;
    private PlayState _play;
    private Editor.Editor _editor;
    private SettingsScreen _settings;
    private Credits _credits;

    private static NonoSharpGame _instance;

    public static void Close()
    {
        _instance.Exit();
    }

    public NonoSharpGame()
    {
        CrashHandler.Initialize();

        // Initialize serilog
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(
                    outputTemplate: "{Timestamp:HH:mm:ss} {Level:u4} {Message:lj}{NewLine}"
                )
            .WriteTo.File(
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Logs", "nonoSharp.log"),
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

        Settings.Initialize();
        StringManager.Initialize();

        _instance = this;
        _state = GameState.MainMenu;
        _perfInfo = new();
        _showPerformanceInfo = false;
        IsMouseVisible = true;
        _mainMenu = new();
        _levelSelect = new();
        _editor = new();
        _play = new();
        _settings = new();
        _credits = new();

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

        _showBgGrid = Settings.GetBool("showBgGrid");

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

        // load textures
        Tile.LoadTextures(Content);
        UI.CheckBox.LoadTextures(Content);

        // load sounds
        Tile.LoadSounds(Content);

        TextRenderer.LoadFont("DefaultFont", "DefaultFont", Content); // load font

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
                _play.Update(_mouse, _mouseOld, _kb, _kbOld, GraphicsDevice, out bool leave, IsActive);
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
                // settings button
                else if (_mainMenu.SettingsButton.IsClicked)
                    _state = GameState.Settings;

                break;
            case GameState.LevelSelect:
                {
                    LevelMetadata levelMetadata = new();
                    _levelSelect.Update(_mouse, _mouseOld, _kb, _kbOld, GraphicsDevice, out GameState? newState, ref levelMetadata);

                    if (newState == GameState.Game)
                    {
                        Mouse.SetPosition(GraphicsDevice.Viewport.Bounds.Width / 2, GraphicsDevice.Viewport.Bounds.Height / 2); // put mouse in middle of screen
                        _play.Load(levelMetadata.GetPath());
                    }
                    if (newState != null)
                        _state = (GameState)newState;

                    break;
                }
            case GameState.Editor:
                {
                    _editor.Update(_mouse, _mouseOld, _kb, _kbOld, GraphicsDevice, out GameState? newState);
                    if (newState != null)
                        _state = (GameState)newState;
                    break;
                }
            case GameState.Settings:
                _settings.Update(_mouse, _mouseOld, _kb, _kbOld, GraphicsDevice);
                if (_settings.BackButton.IsClicked)
                {
                    Settings.Save();
                    _showBgGrid = Settings.GetBool("showBgGrid");
                    _state = GameState.MainMenu;
                }
                if (_settings.CreditsButton.IsClicked)
                    _state = GameState.Credits;
                break;
            case GameState.Credits:
                _credits.Update(_mouse, _mouseOld, _kb, _kbOld);
                if (_credits.BackButton.IsClicked)
                    _state = GameState.Settings;
                break;
        }

        // Show/hide perf info when pressing F12
        if (_kb.IsKeyDown(Keys.F12) && !_kbOld.IsKeyDown(Keys.F12))
            _showPerformanceInfo = !_showPerformanceInfo;

        if (_showPerformanceInfo)
            _perfInfo.UpdatePerformanceInfo();

        // Toggle fullscreen with F11
        if (_kb.IsKeyDown(Keys.F11) && !_kbOld.IsKeyDown(Keys.F11))
            toggleFullScreen();

        Mouse.SetCursor(Cursor);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();

        drawBackgroundGrid();

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
            case GameState.Settings:
                _settings.Draw(_spriteBatch);
                break;
            case GameState.Credits:
                _credits.Draw(_spriteBatch);
                break;
        }

        // draw some performance info
        if (_showPerformanceInfo)
            _perfInfo.Draw(_spriteBatch);

        _spriteBatch.End();

        _perfInfo.UpdateFPS(gameTime);

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

    private void toggleFullScreen()
    {
        if (_graphics.IsFullScreen)
            setWindowSize((int)(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width * 0.85f), (int)(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height * 0.85f), false);
        else
            setWindowSize(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height, true);
    }

    private void drawBackgroundGrid()
    {
        if (!_showBgGrid)
            return;
        GridRenderer.DrawGrid(
                _spriteBatch,
                (int)((_mouse.X - (GraphicsDevice.Viewport.Bounds.Width / 2)) * 0.03f) - 100,
                (int)((_mouse.Y - (GraphicsDevice.Viewport.Bounds.Height / 2)) * 0.03f) - 100,
                (GraphicsDevice.Viewport.Bounds.Height / 32) + 101,
                (GraphicsDevice.Viewport.Bounds.Width / 32) + 101,
                32,
                Settings.GetDarkAccentColor(0.7f));
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
