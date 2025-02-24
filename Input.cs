using Microsoft.Xna.Framework.Input;
using System;

namespace Suburbs;

public static class Input
{
    public static event Action<Keys> OnKeyPressed;

    // TODO: Improve this Input a bit so that it supports multiple keys input

    private static KeyboardState currentKeyboardState;
    private static KeyboardState prevKeyboardState;
    public static void Update()
    {
        currentKeyboardState = Keyboard.GetState();

        foreach (Keys key in currentKeyboardState.GetPressedKeys())
        {
            if (!prevKeyboardState.IsKeyDown(key))
            {
                OnKeyPressed?.Invoke(key);
            }
        }
        
        prevKeyboardState = Keyboard.GetState();
    }
}
