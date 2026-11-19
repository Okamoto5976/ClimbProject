using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetTest_NetworkMovingObjectManager : NetworkBehaviour
{
    [SerializeField] private GameObject _movingObjectPrefab;
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _radius = 3f;

    private List<NetworkObject> _objects = new();

    private Enum_MovingMode _currentMode;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        CreateObjects(Enum_MovingMode.FollowHost);
    }

    // 動作モードを変更
    public void CreateObjects(Enum_MovingMode mode)
    {
        if (!IsServer) return;

        ClearObjects();

        _currentMode = mode;

        switch (mode)
        {
            case Enum_MovingMode.FollowHost:
                CreateObject(mode, NetworkManager.ServerClientId);
                break;

            case Enum_MovingMode.FollowClients:
                foreach (ulong clientId in NetworkManager.ConnectedClientsIds)
                {
                    CreateObject(mode, clientId);
                }
                break;

            case Enum_MovingMode.Clockwise:
                CreateObject(mode, 0);
                break;

            case Enum_MovingMode.FollowInOrder:
                CreateObject(mode, 0);
                break;
        }
    }

    private void CreateObject(Enum_MovingMode mode, ulong targetClientId)
    {
        GameObject obj = Instantiate(_movingObjectPrefab, Vector3.zero, Quaternion.identity);

        NetTest_ServerDrivenObject controller = obj.GetComponent<NetTest_ServerDrivenObject>();

        controller.Initialize(mode, targetClientId, _moveSpeed, _radius);

        NetworkObject networkObject = obj.GetComponent<NetworkObject>();

        // Host所有のNetworkObjectとして生成
        networkObject.Spawn();

        _objects.Add(networkObject);
    }

    private void ClearObjects()
    {
        foreach (var obj in _objects)
        {
            if (obj != null && obj.IsSpawned)
            {
                obj.Despawn(true);
            }
        }

        _objects.Clear();
    }

    public void SetMoveSpeed(float value)
    {
        if (!IsServer) return;

        _moveSpeed = value;

        CreateObjects(_currentMode);
    }

    public void SetRadius(float value)
    {
        if (!IsServer) return;

        _radius = value;

        CreateObjects(_currentMode);
    }

    public void SetMode(Enum_MovingMode mode)
    {
        CreateObjects(mode);
    }
}