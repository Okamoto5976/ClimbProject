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
        Debug.Log($"[Connected] ClientID: {clientId}", this);
    }

    private void OnDisconnected(ulong clientId)
    {
        string reason = m_networkManager.DisconnectReason;
        if(string.IsNullOrEmpty(reason))
        {
            reason = "(理由なし：タイムアウト　or 接続ができなかった可能性)";
        }
        Debug.Log($"[Disconnected] ClientID: {clientId} Reason: {reason}", this);
    }
}
    