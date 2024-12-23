using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace NonoSharp.UI;

public class NumberTextBox : TextBox
{
    private int _max;
    private bool _hasMax;

    public NumberTextBox(
        int x, int y, int width, Color fillColor, Color outlineColor, Color textColor,
        Color textColorHover) : base(x, y, width, fillColor, outlineColor, textColor, textColorHover)
    {
        _max = 0;
        _hasMax = false;
    }

    public NumberTextBox(
        int x, int y, int width, Color fillColor, Color outlineColor, Color textColor,
        Color textColorHover, int max) : base(x, y, width, fillColor, outlineColor, textColor, textColorHover)
    {
        SetMaxValue(max);
    }

    public NumberTextBox(
        int x, int y, int width, Color fillColor, Color outlineColor, Color textColor,
        Color textColorHover, Color placeholderColor, Color placeholderColorHover, string placeholder)
        : base(x, y, width, fillColor, outlineColor, textColor, textColorHover, placeholderColor, placeholderColorHover, placeholder)
    {
        _max = 0;
        _hasMax = false;
    }

    public NumberTextBox(
        int x, int y, int width, Color fillColor, Color outlineColor, Color textColor,
        Color textColorHover, Color placeholderColor, Color placeholderColorHover, string placeholder, int max)
        : base(x, y, width, fillColor, outlineColor, textColor, textColorHover, placeholderColor, placeholderColorHover, placeholder)
    {
        SetMaxValue(max);
    }

    public override void UpdateInput(TextInputEventArgs tiea)
    {
        if (Hovered && (char.IsNumber(tiea.Character) || tiea.Key == Keys.Back))
        {
            base.UpdateInput(tiea);
            checkMax();
        }
    }

    public override void Update(MouseState mouse, MouseState mouseOld, KeyboardState keyboard, KeyboardState keyboardOld)
    {
        base.Update(mouse, mouseOld, keyboard, keyboardOld);

        if (Hovered)
        {
            if (mouse.ScrollWheelValue > mouseOld.ScrollWheelValue)
            {
                int newValue = GetNumberValue() + 1;
                if (!_hasMax || newValue <= _max)
                    SetNumberValue(newValue);
            }
            else if (mouse.ScrollWheelValue < mouseOld.ScrollWheelValue)
            {
                int newValue = GetNumberValue() - 1;
                if (newValue >= 0)
                    SetNumberValue(newValue);
            }
        }
    }

    public int GetNumberValue()
    {
        if (!int.TryParse(text, out int number))
            return -1;
        return number;
    }

    public int SetNumberValue(int value)
    {
        text = value.ToString();
    }

    public void SetMaxValue(int value)
    {
        _max = value;
        _hasMax = true;
    }

    private void checkMax()
    {
        if (_hasMax && GetNumberValue() > _max)
        {
            illegalBlink = true;
            BackSpace();
        }
    }
}
