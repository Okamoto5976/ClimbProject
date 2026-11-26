using UnityEngine;

/// <summary>
/// ClientIdからPlayerごとの識別色を作る(全PCで同じ色になる)。
/// </summary>
public static class NetTest_PlayerColor
{
    // 黄金比で色相をずらすと、隣り合うIDでも色が離れる
    private const float HueStep = 0.61803398875f;

    /// <summary>
    /// ClientIdに対応した色を返す。
    /// </summary>
    public static Color Get(ulong clientId)
    {
        float hue = Mathf.Repeat(clientId * HueStep, 1f);

        return Color.HSVToRGB(hue, 0.75f, 1f);
    }
}
