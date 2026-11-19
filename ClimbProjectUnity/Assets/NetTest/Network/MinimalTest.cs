using System;
using UnityEngine;

using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;

using Unity.Networking.Transport.Relay;
using System.Threading.Tasks;


public class RelayMinimalTest : MonoBehaviour
{
    private async void Start()
    {
        try
        {
            // 1. Unity Services初期化
            await UnityServices.InitializeAsync();

            // 2. 匿名認証
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            Debug.Log("Authentication Success");

            // 3. Relay Allocation作成
            Allocation allocation =
                await RelayService.Instance.CreateAllocationAsync(3);

            Debug.Log("Allocation Success");

            // 4. Join Code取得
            string joinCode =
                await RelayService.Instance.GetJoinCodeAsync(
                    allocation.AllocationId
                );

            Debug.Log($"Join Code: {joinCode}");

            // 5. UnityTransport取得
            UnityTransport transport =
                NetworkManager.Singleton.GetComponent<UnityTransport>();

            // 6. Relay情報を設定（今回はUDP）
            transport.SetRelayServerData(
                AllocationUtils.ToRelayServerData(allocation, "udp")
            );

            Debug.Log("Relay Server Data Set");

            // 7. Host開始
            bool result = NetworkManager.Singleton.StartHost();

            Debug.Log($"StartHost Result: {result}");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        //// 1. Unity Services初期化
        //await UnityServices.InitializeAsync();

        //AuthenticationService.Instance.SignedIn += () =>
        //{
        //    print("Sined in:" + AuthenticationService.Instance.PlayerId);
        //};

        ////    // 2. 匿名認証
        //    await AuthenticationService.Instance.SignInAnonymouslyAsync();

        //try
        //{
        //    Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);

        //    string joinCode =
        //        await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        //    Debug.Log($"Join Code: {joinCode}");

        //    //NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
        //    //    allocation.RelayServer.IpV4,
        //    //    (ushort)allocation.RelayServer.Port,
        //    //    allocation.AllocationIdBytes,
        //    //    allocation.Key,
        //    //    allocation.ConnectionData);

        //    //RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

        //    NetworkManager.Singleton.StartHost();
        //}
        //catch (RelayServiceException e)
        //{
        //    Debug.LogException(e);

        //}
    }


    private async Task JoinRelay(string joinCode)
    {
        try
        {
            Debug.Log("Joining Relay with " + joinCode);
            await RelayService.Instance.JoinAllocationAsync(joinCode);
        }catch(RelayServiceException e)
        {
            Debug.LogException(e);
        }
    }
}
