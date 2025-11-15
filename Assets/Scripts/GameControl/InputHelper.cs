using UnityEngine;
using UnityEngine.InputSystem;

public enum InputKey
{
    Up,
    Down,
    Left,
    Right
}

public static class InputHelper
{
    public static InputAction GetInputAction(InputKey key)
    {
        return InputSystem.actions.FindAction(InputKeyToActionName(key));
    }

    private static string InputKeyToActionName(InputKey key)
    {
        return key switch
        {
            InputKey.Up => "Up",
            InputKey.Down => "Down",
            InputKey.Left => "Left",
            InputKey.Right => "Right",
            _ => throw new System.NotImplementedException(),
        };
    }
}
