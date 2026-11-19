using Unity.Netcode;
using UnityEngine;

public class NetworkDebug : MonoBehaviour
{
    private NetworkManager m_networkManager;

    private void Awake()
    {
        m_networkManager = GetComponent<NetworkManager>();
    }

    private void OnEnable()
    {
        m_networkManager.OnClientConnectedCallback += OnConnected;
        m_networkManager.OnClientDisconnectCallback += OnDisconnected;
    }

    private void OnDisable()
    {
        m_networkManager.OnClientConnectedCallback -= OnConnected;
        m_networkManager.OnClientDisconnectCallback -= OnDisconnected;
    }

    private void OnConnected(ulong clientId)
    {
        Debug.Log($"[Connected] ClientID: {clientId}");
    }

    private void OnDisconnected(ulong clientId)
    {
        Debug.Log($"[Disconnected] ClientID: {clientId}");
    }
}
