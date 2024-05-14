using Microsoft.Xna.Framework.Input;

namespace NonoSharp;

public static class KeyboardStateExtensions
{
    public static bool IsCombinationPressed(this KeyboardState state, KeyboardState oldState, params Keys[] keys)
    {
        // all keys have to be pressed in the current state
        foreach (var key in keys)
        {
            if (!state.IsKeyDown(key))
            {
                return false;
            }
        }

        // at least one of the keys has to be pressed in the old state
        foreach (var key in keys)
        {
            if (!oldState.IsKeyDown(key))
            {
                return true;
            }
        }

        return false;
    }
}
