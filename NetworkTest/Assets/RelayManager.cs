using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Networking.Transport.Relay;
using System.Threading.Tasks;

public class RelayManager : MonoBehaviour
{
    public async Task<string> CreateHost()
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        // 最大接続人数（Host以外）
        Allocation allocation =
            await RelayService.Instance.CreateAllocationAsync(3);

        var transport =
            NetworkManager.Singleton.GetComponent<UnityTransport>();

        transport.SetRelayServerData(
            AllocationUtils.ToRelayServerData(allocation, "dtls")
        );

        string joinCode =
            await RelayService.Instance.GetJoinCodeAsync(
                allocation.AllocationId
            );

        if (!NetworkManager.Singleton.StartHost())
        {
            throw new System.Exception("Hostの開始に失敗しました");
        }

        return joinCode;
    }

    public async Task<bool> JoinHost(string joinCode)
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        JoinAllocation allocation =
            await RelayService.Instance.JoinAllocationAsync(joinCode);

        var transport =
            NetworkManager.Singleton.GetComponent<UnityTransport>();

        transport.SetRelayServerData(
            AllocationUtils.ToRelayServerData(allocation, "dtls")
        );

        return NetworkManager.Singleton.StartClient();
    }
}
