using Microsoft.Xna.Framework;

namespace NonoSharp.UI;

public class NumberTextBox : TextBox
{
    public NumberTextBox(
        int x, int y, int width, Color fillColor, Color outlineColor, Color textColor,
        Color textColorHover) : base(x, y, width, fillColor, outlineColor, textColor, textColorHover)
    {
    }

    public override void UpdateInput(object sender, TextInputEventArgs tiea)
    {
        if (Hovered && char.IsNumber(tiea.Character))
            base.UpdateInput(sender, tiea);
    }

    public int GetNumberValue()
    {
        int number = 0;
        if (!int.TryParse(Text, out number))
            return -1;
        return number;
    }
}
