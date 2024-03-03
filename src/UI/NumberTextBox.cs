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

    public override void UpdateInput(object sender, TextInputEventArgs tiea)
    {
        if (Hovered && (char.IsNumber(tiea.Character) || tiea.Key == Keys.Back))
        {
            base.UpdateInput(sender, tiea);
            checkMax();
        }
    }

    public int GetNumberValue()
    {
        int number;
        if (!int.TryParse(Text, out number))
            return -1;
        return number;
    }

    public void SetMaxValue(int value)
    {
        _max = value;
        _hasMax = true;
    }

    private void checkMax()
    {
        if (_hasMax && GetNumberValue() > _max)
            BackSpace();
    }
}
