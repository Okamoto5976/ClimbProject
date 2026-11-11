using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkStart : MonoBehaviour
{
    private const ushort Port = 7777;

    [ContextMenu("Test HostStart")]
    public void TestHostStart()
    {
        StartHost();
    }

    public void StartHost()
    {
        UnityTransport transport =
            NetworkManager.Singleton.GetComponent<UnityTransport>();

        //ê⁄ë±ÇéÛÇØïtÇØÇÈ
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
        UnityTransport transport =
            NetworkManager.Singleton.GetComponent<UnityTransport>();

        transport.SetConnectionData(
            hostAddress,
            Port
            );

        NetworkManager.Singleton.StartClient();
    }
}
