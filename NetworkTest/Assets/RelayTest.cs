using System;
using System.Threading.Tasks;
using UnityEngine;

using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;

using Unity.Networking.Transport.Relay;
//using Unity.Networking.Transport.Relay.AllocationUtils;

public class RelayTestUI : MonoBehaviour
{
    private string m_joinCode = "";
    private string m_inputCode = "";
    private string m_status = "Ready";

    private bool m_isBusy;

    private void Start()
    {
        // 接続イベントを登録
        NetworkManager.Singleton.OnClientConnectedCallback
            += OnClientConnected;

        NetworkManager.Singleton.OnClientDisconnectCallback
            += OnClientDisconnected;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback
                -= OnClientConnected;

            NetworkManager.Singleton.OnClientDisconnectCallback
                -= OnClientDisconnected;
        }
    }

    // Unity Servicesの初期化と匿名認証
    private async Task InitializeServices()
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    // Host開始
    private async Task CreateHost()
    {
        try
        {
            m_isBusy = true;
            m_status = "Creating Host...";

            await InitializeServices();

            // Relayの接続枠を1人分確保
            Allocation allocation =
                await RelayService.Instance.CreateAllocationAsync(1);

            // Join Codeを取得
            m_joinCode =
                await RelayService.Instance.GetJoinCodeAsync(
                    allocation.AllocationId
                );

            // Unity TransportにRelay情報を設定
            UnityTransport transport =
                NetworkManager.Singleton.GetComponent<UnityTransport>();

            transport.SetRelayServerData(
                AllocationUtils.ToRelayServerData(allocation, "dtls")
            );

            // Host開始
            var manager = NetworkManager.Singleton;

            Debug.Log($"IsListening: {manager.IsListening}");
            Debug.Log($"IsClient: {manager.IsClient}");
            Debug.Log($"IsServer: {manager.IsServer}");
            Debug.Log($"Transport: {manager.NetworkConfig.NetworkTransport}");

            bool result = manager.StartHost();

            Debug.Log($"StartHost Result: {result}");

            if (!result)
            {
                m_status = "Host Start Failed";
                return;
            }

            m_status = "Host Started!";
        }
        catch (Exception e)
        {
            m_status = e.Message;
            Debug.LogException(e);
        }
        finally
        {
            m_isBusy = false;
        }
    }

    // Client接続
    private async Task JoinHost()
    {
        try
        {
            m_isBusy = true;
            m_status = "Joining Host...";

            await InitializeServices();

            // Join CodeからRelay情報を取得
            JoinAllocation allocation =
                await RelayService.Instance.JoinAllocationAsync(
                    m_inputCode.Trim()
                );

            // Unity TransportにRelay情報を設定
            UnityTransport transport =
                NetworkManager.Singleton.GetComponent<UnityTransport>();

            transport.SetRelayServerData(
                AllocationUtils.ToRelayServerData(allocation, "dtls")
            );

            // Client開始
            bool result = NetworkManager.Singleton.StartClient();

            if (!result)
            {
                throw new Exception("Clientの開始に失敗しました");
            }

            m_status = "Client Started!";
        }
        catch (Exception e)
        {
            m_status = e.Message;
            Debug.LogException(e);
        }
        finally
        {
            m_isBusy = false;
        }
    }

    // 接続成功イベント
    private void OnClientConnected(ulong clientId)
    {
        m_status = $"Connected! Client ID: {clientId}";
        Debug.Log(m_status);
    }

    // 切断イベント
    private void OnClientDisconnected(ulong clientId)
    {
        m_status = $"Disconnected: Client ID: {clientId}";
        Debug.LogWarning(m_status);
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 350, 350), GUI.skin.box);

        GUILayout.Label("Relay Test");
        GUILayout.Space(10);

        GUI.enabled = !m_isBusy &&
                      !NetworkManager.Singleton.IsListening;

        if (GUILayout.Button("Start Host", GUILayout.Height(40)))
        {
            _ = CreateHost();
        }

        GUILayout.Space(10);

        GUILayout.Label("Join Code:");
        GUILayout.Label(m_joinCode);

        GUILayout.Space(10);

        GUILayout.Label("Enter Join Code:");
        m_inputCode = GUILayout.TextField(m_inputCode);

        if (GUILayout.Button("Start Client", GUILayout.Height(40)))
        {
            _ = JoinHost();
        }

        GUI.enabled = true;

        GUILayout.Space(20);
        GUILayout.Label("Status:");
        GUILayout.Label(m_status);

        GUILayout.EndArea();
    }
}
