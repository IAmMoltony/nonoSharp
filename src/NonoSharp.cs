using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace NonoSharp;

public class NonoSharpGame : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private int _boardSize;
    private Tile[,] _board;
    private MouseState _mouse;
    private MouseState _mouseOld;

    private void makeBoard()
    {
        _board = new Tile[_boardSize, _boardSize];
        for (int i = 0; i < _boardSize; i++)
            for (int j = 0; j < _boardSize; j++)
                _board[i, j] = new Tile();
    }

    public NonoSharpGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _boardSize = 5;
        makeBoard();
    }

    protected override void Initialize()
    {
        _graphics.IsFullScreen = false;
        _graphics.PreferredBackBufferWidth = 800;
        _graphics.PreferredBackBufferHeight = 600;
        _graphics.ApplyChanges();
        Window.AllowUserResizing = true;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        RectRenderer.Load(GraphicsDevice);
        Tile.LoadTextures(Content);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _mouseOld = _mouse;
        _mouse = Mouse.GetState();
        int mx = _mouse.X;
        int my = _mouse.Y;
        for (int i = 0; i < _boardSize; i++)
            for (int j = 0; j < _boardSize; j++)
            {
                ref Tile tile = ref _board[i, j];
                tile.Hover(i, j, mx, my, _boardSize, GraphicsDevice);
                if (tile.IsHovered)
                {
                    if (_mouseOld.LeftButton == ButtonState.Released && _mouse.LeftButton == ButtonState.Pressed)
                        tile.LeftClick();
                    if (_mouseOld.RightButton == ButtonState.Released && _mouse.RightButton == ButtonState.Pressed)
                        tile.RightClick();
                }
            }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();
        for (int i = 0; i < _boardSize; i++)
            for (int j = 0; j < _boardSize; j++)
                _board[i, j].Draw(i, j, _boardSize, _spriteBatch, GraphicsDevice);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
