using Microsoft.Xna.Framework;
using System;

namespace NonoSharp;

public static class ColorExtensions
{
    public static Color Darker(this Color color, float howMuch)
    {
        // Make sure `howMuch' is 0.0 to 1.0
        howMuch = Math.Max(0.0f, Math.Min(1.0f, howMuch));

        // Calculate darker color
        int r = (int)(color.R * (1 - howMuch));
        int g = (int)(color.G * (1 - howMuch));
        int b = (int)(color.B * (1 - howMuch));

        return new(r, g, b);
    }
}
