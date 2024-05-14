using Microsoft.Xna.Framework;
using System;

namespace NonoSharp;

/// <summary>
/// A class for extensions of the <c>Microsoft.Xna.Framework.Color</c> class.
/// </summary>
public static class ColorExtensions
{
    /// <summary>
    /// Get a darker version of a color.
    /// </summary>
    /// <param name="color">The color to get a dark version of</param>
    /// <param name="howMuch">How much darker to make the color (0.0 to 1.0 inclusive)</param>
    /// <returns>A darker version of the color</returns>
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

    /// <summary>
    /// Get a lighter version of a color.
    /// </summary>
    /// <param name="color">The color to get a light version of</param>
    /// <param name="howMuch">How much lighter to make the color (0.0 to 1.0 inclusive)</param>
    /// <returns>A lighter version of the color</returns>
    public static Color Lighter(this Color color, float howMuch)
    {
        // Make sure `howMuch' is 0.0 to 1.0
        howMuch = Math.Max(0.0f, Math.Min(1.0f, howMuch));

        // Calculate lighter color
        int r = (int)(color.R + (255 - color.R) * howMuch);
        int g = (int)(color.G + (255 - color.G) * howMuch);
        int b = (int)(color.B + (255 - color.B) * howMuch);

        return new(r, g, b);
    }

    /// <summary>
    /// Check if the color is light.
    /// </summary>
    /// <param name="color">The color to check</param>
    /// <returns>Whether or not the color is light</returns>
    public static bool IsLight(this Color color)
    {
        int brightness = (int)color.R + (int)color.G + (int)color.B;
        return brightness >= 383;
    }
}
