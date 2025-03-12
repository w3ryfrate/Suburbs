using Microsoft.Xna.Framework.Input;
using System;

namespace Suburbs;

public static class Input
{
    public static event Action<Keys> OnKeyPressed;

    // TODO: Improve this Input a bit so that it supports multiple keys input

    private static KeyboardState _currentKeyboardState;
    private static KeyboardState _prevKeyboardState;
    public static void Update()
    {
        _currentKeyboardState = Keyboard.GetState();

        foreach (Keys key in _currentKeyboardState.GetPressedKeys())
        {
            if (!_prevKeyboardState.IsKeyDown(key))
            {
                OnKeyPressed?.Invoke(key);
            }
        }
        
        _prevKeyboardState = Keyboard.GetState();
    }
}
