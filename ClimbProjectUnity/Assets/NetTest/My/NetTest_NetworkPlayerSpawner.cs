using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetTest_NetworkPlayerSpawner : NetworkBehaviour
{
    [SerializeField]
    private GameObject _playerPrefab;

    [SerializeField]
    private float _spawnInterval = 2f;

    private Dictionary<ulong, NetworkObject> _players = new();

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        NetworkManager.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.OnClientDisconnectCallback += OnClientDisconnected;

        // Ç∑Ç≈Ç…ê⁄ë±ÇµÇƒÇ¢ÇÈPlayerÇê∂ê¨
        foreach (ulong clientId in NetworkManager.ConnectedClientsIds)
        {
            SpawnPlayer(clientId);
        }
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;

        NetworkManager.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.OnClientDisconnectCallback -= OnClientDisconnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        SpawnPlayer(clientId);
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (!_players.TryGetValue(clientId, out NetworkObject player))
            return;

        if (player != null && player.IsSpawned)
        {
            player.Despawn(true);
        }

        _players.Remove(clientId);
    }

    private void SpawnPlayer(ulong clientId)
    {
        if (_players.ContainsKey(clientId))
            return;

        Vector3 spawnPosition = new Vector3(clientId * _spawnInterval, 0f, 0f);

        GameObject obj = Instantiate(_playerPrefab, spawnPosition, Quaternion.identity);

        NetworkObject networkObject = obj.GetComponent<NetworkObject>();

        // PlayerÇÃèäóLé“Çê›íË
        networkObject.SpawnAsPlayerObject(clientId, true);

        _players.Add(clientId, networkObject);

        Debug.Log($"Player Spawn : {clientId}", this);
    }
}