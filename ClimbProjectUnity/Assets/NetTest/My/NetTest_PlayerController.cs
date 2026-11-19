using Unity.Netcode;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class NetTtest_PlayerController : NetworkBehaviour
{
    [SerializeField]
    private float _moveSpeed = 5f;

    private void Update()
    {
        // 自分のPlayer以外は操作できない
        if (!IsOwner) return;

        Vector2 input = ReadInput();

        Vector3 move = new Vector3(input.x, 0f, input.y);

        if (move.sqrMagnitude > 1f) move.Normalize();

        transform.position += move * _moveSpeed * Time.deltaTime;
    }

    private Vector2 ReadInput()
    {
        Vector2 v = Vector2.zero;

#if ENABLE_INPUT_SYSTEM

        var kb = Keyboard.current;

        if (kb == null) return v;

        if (kb.wKey.isPressed) v.y += 1f;
        if (kb.sKey.isPressed) v.y -= 1f;
        if (kb.dKey.isPressed) v.x += 1f;
        if (kb.aKey.isPressed) v.x -= 1f;

#else

        if (Input.GetKey(KeyCode.W)) v.y += 1f;
        if (Input.GetKey(KeyCode.S)) v.y -= 1f;
        if (Input.GetKey(KeyCode.D)) v.x += 1f;
        if (Input.GetKey(KeyCode.A)) v.x -= 1f;

#endif

        return v;
    }
}