using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public abstract class UIElement
{
    public int x, y;
    
    public UIElement(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public abstract void Draw(SpriteBatch sprBatch);
    public abstract void Update(MouseState mouse, MouseState mouseOld, KeyboardState keyboard, KeyboardState keyboardOld);
}
