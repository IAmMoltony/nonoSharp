using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace NonoSharp;

public static class KeyboardStateExtensions
{
    public static bool IsCombinationPressed(this KeyboardState state, KeyboardState oldState, params Keys[] keys)
    {
        return keys.All(key => state.IsKeyDown(key)) && keys.Any(key => !oldState.IsKeyDown(key));
    }

    public static bool AnyKeyPressed(this KeyboardState state)
    {
        return state.GetPressedKeys().Length > 0;
    }
}
