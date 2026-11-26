using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// 接続したクライアントごとにPlayerを生成し、切断時に片付ける（Server専用）。
/// </summary>
public class NetTest_NetworkPlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private float _spawnInterval = 2f;

    private readonly Dictionary<ulong, NetworkObject> _players = new();

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        NetworkManager.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.OnClientDisconnectCallback += OnClientDisconnected;

        // すでに接続しているPlayerを生成
        foreach (ulong clientId in NetworkManager.ConnectedClientsIds)
        {
            SpawnPlayer(clientId);
        }
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;

        if (NetworkManager != null)
        {
            NetworkManager.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.OnClientDisconnectCallback -= OnClientDisconnected;
        }

        _players.Clear();
    }

    private void OnClientConnected(ulong clientId)
    {
        SpawnPlayer(clientId);
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (!_players.Remove(clientId, out NetworkObject player)) return;

        // Netcode側で既に破棄されている場合があるためnull判定する
        if (player != null && player.IsSpawned)
        {
            player.Despawn(true);
        }
    }

    private void SpawnPlayer(ulong clientId)
    {
        if (!IsServer) return;
        // すでにPlayerObjectが存在する場合は生成しない
        if (NetworkManager.ConnectedClients.TryGetValue(clientId, out var client))
        {
            if (client.PlayerObject != null)return;
        }
        if (_players.ContainsKey(clientId)) return;

        Vector3 spawnPosition = new Vector3(clientId * _spawnInterval, 0f, 0f);

        GameObject obj = Instantiate(_playerPrefab, spawnPosition, Quaternion.identity);

        NetworkObject networkObject = obj.GetComponent<NetworkObject>();

        // Playerの所有者を設定
        networkObject.SpawnAsPlayerObject(clientId, true);

        _players.Add(clientId, networkObject);

        Debug.Log($"Player Spawn : {clientId}", this);
    }
}