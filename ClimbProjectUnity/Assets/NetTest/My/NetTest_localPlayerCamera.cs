using Unity.Netcode;
using UnityEngine;

/// <summary>
/// 各PCのローカルPlayerを追従するカメラ
/// </summary>
public class NetTest_LocalPlayerCamera : MonoBehaviour
{
    [SerializeField] private Vector3 _offset = new Vector3(0f, 0f, -10f);

    private Transform _target;

    private void LateUpdate()
    {
        // ターゲットが存在しない場合は取得する
        if (_target == null)
        {
            FindLocalPlayer();
            return;
        }

        transform.position = _target.position + _offset;
    }

    /// <summary>
    /// 自分が操作するPlayerを取得する
    /// </summary>
    private void FindLocalPlayer()
    {
        if (NetworkManager.Singleton == null) return;
        //if (!NetworkManager.Singleton.IsClient) return;
        if (!NetworkManager.Singleton.IsConnectedClient && !NetworkManager.Singleton.IsHost) return;

        NetworkObject playerObject = NetworkManager.Singleton.LocalClient.PlayerObject;

        if (playerObject == null) return; 

        _target = playerObject.transform;
    }
}