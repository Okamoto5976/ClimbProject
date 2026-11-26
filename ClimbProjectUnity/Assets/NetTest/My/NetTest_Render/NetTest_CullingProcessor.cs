using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// カメラ範囲(+Offset)外のターゲットを判定してカリング状態を切り替える。
/// 1フレームに調べる数を制限し、ラウンドロビンで全体を巡回して負荷を分散する。
/// (Unity APIはメインスレッド専用のため、async/スレッドではなく時間分割で軽くしている)
/// </summary>
public sealed class NetTest_CullingProcessor
{
    // 表示→非表示になる境界 / 非表示→表示に戻る境界(ビューポート比率)
    private readonly float _showMargin;
    private readonly float _hideMargin;

    private int _cursor;

    /// <param name="showMargin">カメラ外に広げる範囲。この内側なら表示</param>
    /// <param name="hysteresis">ちらつき防止。表示中はshowMargin+この値まで許容する</param>
    public NetTest_CullingProcessor(float showMargin, float hysteresis)
    {
        _showMargin = showMargin;
        _hideMargin = showMargin + hysteresis;
    }

    /// <summary>
    /// 毎フレーム呼ぶ。maxChecks個ぶんだけ判定を進める。
    /// </summary>
    public void Tick(Camera camera,　IReadOnlyList<NetTest_RenderTarget> targets,　int maxChecks)
    {
        int count = targets.Count;

        if (count == 0) return;

        int checks = Mathf.Min(maxChecks, count);

        for (int i = 0; i < checks; i++)
        {
            // 判定中にUnregisterされても範囲外にならないようにする
            if (_cursor >= count)
            {
                _cursor = 0;
            }

            Evaluate(camera, targets[_cursor]);

            _cursor++;
        }
    }

    private void Evaluate(Camera camera, NetTest_RenderTarget target)
    {
        if (target == null) return;

        // 自分自身は常に表示
        if (target.IsLocalPlayer)
        {
            target.SetCulled(false);
            return;
        }

        Vector3 viewport = camera.WorldToViewportPoint(target.transform.position);

        // 現在非表示なら戻りにくく(狭く)、表示中なら外れにくく(広く)する
        float margin = target.IsCulled ? _showMargin : _hideMargin;

        bool isInside =
            viewport.z > 0f &&
            viewport.x > -margin && viewport.x < 1f + margin &&
            viewport.y > -margin && viewport.y < 1f + margin;

        target.SetCulled(!isInside);
    }
}
