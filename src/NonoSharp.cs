using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace NonoSharp;

public class NonoSharpGame : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Board _board;
    private MouseState _mouse;
    private MouseState _mouseOld;
    private FPSCounter _fpsCounter;

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
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _mouseOld = _mouse;
        _mouse = Mouse.GetState();
        _board.Update(_mouse, _mouseOld, GraphicsDevice);
        if (_board.IsSolved)
            _solveTimeTick = false;

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();

        _board.Draw(_spriteBatch, GraphicsDevice);
        TextRenderer.DrawText(_spriteBatch, "notosans", 10, 10, 0.6f, $"Time: {Math.Round(_solveTime / 1000, 2)} s", _board.IsSolved ? Color.Lime : Color.White);
        TextRenderer.DrawText(_spriteBatch, "notosans", 10, GraphicsDevice.Viewport.Bounds.Height - 26, 0.33f, $"{Math.Round(_fpsCounter.CurrentFPS)} fps, {Math.Round(_fpsCounter.AverageFPS)} avg", Color.LightGray);

        _spriteBatch.End();

        _fpsCounter.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

        base.Draw(gameTime);
    }

    protected override void OnExiting(object sender, EventArgs e)
    {
        _solveTimeThreadRunning = false;
        _solveTimeThread.Join();
        base.OnExiting(sender, e);
    }
}
