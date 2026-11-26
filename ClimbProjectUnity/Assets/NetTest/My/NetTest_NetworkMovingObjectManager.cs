using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// 動くオブジェクトの生成・モード切替・移動更新を管理する。
/// Host(Server)専用。
/// </summary>
public class NetTest_NetworkMovingObjectManager : NetworkBehaviour
{
    [Header("Prefab")]
    [SerializeField]
    private GameObject _movingObjectPrefab;

    [Header("Move Settings")]
    [SerializeField]
    private float _moveSpeed = 3f;

    [Header("Random Move Settings")]
    [SerializeField]
    private float _moveInterval = 2f;

    private readonly List<NetTest_MovingMotion> _motions = new();

    private Enum_MovingMode _currentMode =
        Enum_MovingMode.FollowHost;

    private bool _isDirty;

    public float MoveSpeed => _moveSpeed;

    public float MoveInterval => _moveInterval;

    public override void OnNetworkSpawn()
    {
        enabled = IsServer;

        if (!IsServer) return;

        NetworkManager.OnClientConnectedCallback
            += OnClientListChanged;

        NetworkManager.OnClientDisconnectCallback
            += OnClientListChanged;

        ApplyMode(_currentMode);
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;

        if (NetworkManager != null)
        {
            NetworkManager.OnClientConnectedCallback
                -= OnClientListChanged;

            NetworkManager.OnClientDisconnectCallback
                -= OnClientListChanged;
        }

        _motions.Clear();
    }

    private void Update()
    {
        if (_isDirty)
        {
            _isDirty = false;

            ApplyMode(_currentMode);
        }

        float deltaTime = Time.deltaTime;

        for (int i = 0; i < _motions.Count; i++)
        {
            _motions[i].Tick(deltaTime);
        }
    }

    /// <summary>
    /// 移動モードを変更
    /// </summary>
    public void SetMode(Enum_MovingMode mode)
    {
        if (!IsServer) return;

        ApplyMode(mode);
    }

    /// <summary>
    /// 移動速度を変更
    /// </summary>
    public void SetMoveSpeed(float value)
    {
        if (!IsServer) return;

        _moveSpeed = value;

        for (int i = 0; i < _motions.Count; i++)
        {
            _motions[i].SetMoveSpeed(value);
        }
    }

    /// <summary>
    /// 移動方向の変更間隔を変更
    /// </summary>
    public void SetMoveInterval(float value)
    {
        if (!IsServer) return;

        _moveInterval = Mathf.Max(0.01f, value);

        for (int i = 0; i < _motions.Count; i++)
        {
            _motions[i].SetMoveInterval(_moveInterval);
        }
    }

    /// <summary>
    /// Clientの接続・切断時
    /// </summary>
    private void OnClientListChanged(ulong clientId)
    {
        if (_currentMode == Enum_MovingMode.FollowClients)
        {
            _isDirty = true;
        }
    }

    /// <summary>
    /// 移動モードを適用する
    /// </summary>
    private void ApplyMode(Enum_MovingMode mode)
    {
        _currentMode = mode;

        IReadOnlyList<ulong> clientIds =
            NetworkManager.ConnectedClientsIds;

        int required =
            mode == Enum_MovingMode.FollowClients
            ? clientIds.Count
            : 1;

        while (_motions.Count < required)
        {
            SpawnMotion();
        }

        while (_motions.Count > required)
        {
            DespawnLastMotion();
        }

        for (int i = 0; i < _motions.Count; i++)
        {
            ulong targetId = mode switch
            {
                Enum_MovingMode.FollowHost =>
                    NetworkManager.ServerClientId,

                Enum_MovingMode.FollowClients =>
                    clientIds[i],

                _ => 0UL
            };

            _motions[i].Setup(
                mode,
                targetId,
                _moveSpeed,
                _moveInterval
            );
        }
    }

    /// <summary>
    /// 敵を生成する
    /// </summary>
    private void SpawnMotion()
    {
        GameObject obj = Instantiate(
            _movingObjectPrefab,
            Vector3.zero,
            Quaternion.identity
        );

        NetworkObject networkObject =
            obj.GetComponent<NetworkObject>();

        // Host所有のNetworkObjectとして生成
        networkObject.Spawn();

        _motions.Add(
            new NetTest_MovingMotion(
                NetworkManager,
                networkObject
            )
        );
    }

    /// <summary>
    /// 最後の敵を削除する
    /// </summary>
    private void DespawnLastMotion()
    {
        int last = _motions.Count - 1;

        NetworkObject networkObject =
            _motions[last].NetworkObject;

        _motions.RemoveAt(last);

        if (networkObject != null &&
            networkObject.IsSpawned)
        {
            networkObject.Despawn(true);
        }
    }
}