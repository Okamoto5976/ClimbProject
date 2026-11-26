using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// カリング対象の登録簿(そのPC内だけのローカルな情報)。
/// 削除は末尾と入れ替えるのでO(1)。順序は保証しない。
/// </summary>
public static class NetTest_RenderRegistry
{
    private static readonly List<NetTest_RenderTarget> _targets = new();
    private static readonly List<NetTest_RenderTarget> _indicatorTargets = new();

    /// <summary>登録されている全ターゲット</summary>
    public static IReadOnlyList<NetTest_RenderTarget> Targets => _targets;

    /// <summary>インジケーター表示対象のみ</summary>
    public static IReadOnlyList<NetTest_RenderTarget> IndicatorTargets => _indicatorTargets;

    /// <summary>
    /// ターゲットを登録する。二重登録は無視。
    /// </summary>
    public static void Register(NetTest_RenderTarget target)
    {
        if (target == null || _targets.Contains(target)) return;

        _targets.Add(target);

        if (target.ShowIndicator)
        {
            _indicatorTargets.Add(target);
        }
    }

    /// <summary>
    /// ターゲットの登録を解除する。
    /// </summary>
    public static void Unregister(NetTest_RenderTarget target)
    {
        RemoveSwap(_targets, target);
        RemoveSwap(_indicatorTargets, target);
    }

    private static void RemoveSwap(List<NetTest_RenderTarget> list, NetTest_RenderTarget item)
    {
        int index = list.IndexOf(item);

        if (index < 0) return;

        int last = list.Count - 1;

        list[index] = list[last];
        list.RemoveAt(last);
    }

    // ドメインリロード無効時にstaticが残らないようにする
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetOnLoad()
    {
        _targets.Clear();
        _indicatorTargets.Clear();
    }
}
