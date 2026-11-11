using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkStart : MonoBehaviour
{
    private const ushort Port = 7777;

    [SerializeField] private string m_hostAddress = "10.219.32.121";

    [ContextMenu("Test HostStart")]
    public void TestHostStart()
    {
        StartHost();
    }

    [ContextMenu("Test ClientStart")]
    public void TestClientStart()
    {
        StartClient(m_hostAddress);
    }

    public void StartHost()
    {
      

        UnityTransport transport =
            NetworkManager.Singleton.GetComponent<UnityTransport>();

        //接続を受け付ける
        transport.SetConnectionData(
            "127.0.0.1",
            Port,
            "0.0.0.0"
            );

        NetworkManager.Singleton.StartHost();
    }

    //10.219.32.121 HostAddress

    public void StartClient(string hostAddress)
    {
        var manager = NetworkManager.Singleton;

        Debug.Log($"IsListening: {manager.IsListening}");
        Debug.Log($"IsHost: {manager.IsHost}");
        Debug.Log($"IsClient: {manager.IsClient}");
        Debug.Log($"IsServer: {manager.IsServer}");

        if (manager.IsListening)
        {
            Debug.LogError("すでに通信が開始されています！");
            return;
        }


        UnityTransport transport =
            NetworkManager.Singleton.GetComponent<UnityTransport>();

        transport.SetConnectionData(
            hostAddress,
            Port
            );

        bool result = NetworkManager.Singleton.StartClient();

        Debug.Log($"StartClient Result: {result}");
    }
}
