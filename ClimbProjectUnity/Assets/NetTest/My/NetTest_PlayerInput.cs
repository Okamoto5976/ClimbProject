#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

using UnityEngine;

/// <summary>
/// Debug用の入力判定クラス。
/// </summary>
public static class NetTest_PlayerInput
{
    /// <summary>
    /// WASDの入力をVector2で返す。
    /// X：左右 / Y：上下
    /// </summary>
    public static Vector2 ReadMove()
    {
        Vector2 v = Vector2.zero;

#if ENABLE_INPUT_SYSTEM

        Keyboard current = Keyboard.current;

        if (current == null) return v;

        if (current.wKey.isPressed) v.y += 1f;
        if (current.sKey.isPressed) v.y -= 1f;
        if (current.dKey.isPressed) v.x += 1f;
        if (current.aKey.isPressed) v.x -= 1f;

#else

        if (Input.GetKey(KeyCode.W)) v.y += 1f;
        if (Input.GetKey(KeyCode.S)) v.y -= 1f;
        if (Input.GetKey(KeyCode.D)) v.x += 1f;
        if (Input.GetKey(KeyCode.A)) v.x -= 1f;

#endif

        return Vector2.ClampMagnitude(v, 1f);
    }
}