using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Serilog;
using System;
using System.IO;
using System.Diagnostics;

namespace NonoSharp;

public interface IGameState
{
    void Draw(SpriteBatch sprBatch);
    IGameState? Update(MouseState mouse, MouseState mouseOld, KeyboardState kb, KeyboardState kbOld, GraphicsDevice graphDev, ref LevelMetadata levelMetadata, bool hasFocus);
}

public class NonoSharpGame : Game
{
    public static MouseCursor Cursor { get; set; } = MouseCursor.Arrow;

    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Process _gameProcess;

    private MouseState _mouse;
    private MouseState _mouseOld;

    private KeyboardState _kb;
    private KeyboardState _kbOld;

    private FPSCounter _fpsCounter;
    private bool _showFPS;

    private IGameState _currentState;
    
    private MainMenu _mainMenu;
    private Editor.Editor _editor;

    public NonoSharpGame()
    {
        CrashHandler.Initialize(Exit);

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

        _gameProcess = Process.GetCurrentProcess();

        Settings.Initialize();
        StringManager.Initialize();

        _fpsCounter = new();
        _showFPS = false;
        IsMouseVisible = true;
        _mainMenu = new();
        _currentState = _mainMenu;
        _editor = new();

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

        LevelMetadata levelMetadata = new();
        if (_currentState.Update(_mouse, _mouseOld, _kb, _kbOld, GraphicsDevice,
                ref levelMetadata, IsActive) is { } newState)
        {
            _currentState = newState;
        }
        if (_currentState == _mainMenu && _mainMenu.QuitButton.IsClicked)
        {
            Exit();
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

        drawBackgroundGrid();

        _currentState.Draw(_spriteBatch);

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
        if (_currentState == _editor)
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

    private void drawBackgroundGrid()
    {
        if (!Settings.GetBool("showBgGrid"))
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
