using Unity.Netcode.Components;

/// <summary>
/// 所有者(オーナー)が位置を決めて同期するNetworkTransform。
/// 標準のNetworkTransformはサーバー権限のため、クライアントの移動が反映されない。
/// </summary>
public class NetTest_ClientNetworkTransform : NetworkTransform
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
