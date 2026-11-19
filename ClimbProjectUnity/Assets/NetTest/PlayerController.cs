using Unity.Netcode;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// プレイヤーPrefabにアタッチ。
/// 自分が所有するオブジェクト(IsOwner)だけをWASDで動かす。
/// </summary>
public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float m_moveSpeed = 5f;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            // Host/Clientの生成位置が重ならないようにずらす
            transform.position = new Vector3(OwnerClientId * 2f, 0f, 0f);
        }
    }

    private void Update()
    {
        if (!IsOwner) return;

        Vector2 input = ReadInput();
        Vector3 move = new Vector3(input.x, 0f, input.y);

        if (move.sqrMagnitude > 1f) move.Normalize();

        transform.position += move * m_moveSpeed * Time.deltaTime;
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
