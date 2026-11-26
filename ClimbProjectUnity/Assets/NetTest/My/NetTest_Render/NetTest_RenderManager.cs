using Unity.Netcode;
using UnityEngine;

/// <summary>
/// そのPC(Host / Client問わず)専用の描画負荷軽減マネージャー。
/// ・カメラ範囲+Offset外のEntityを非表示にして、見た目の処理を止める
/// ・画面外にいる仲間の方向を、色付きアイコン＋矢印で画面の縁に表示する
///
/// ネットワーク上の状態は一切変更しない(他PCの画面には影響しない)。
/// 実処理はピュアクラスに任せ、このクラスは更新を呼ぶだけの存在
/// Awakeは使わず、OnEnable / OnDisableで生成・破棄する。
/// </summary>
[DisallowMultipleComponent]
public class NetTest_RenderManager : MonoBehaviour
{
    [Header("Camera")]
    [Tooltip("未設定ならCamera.mainを使う")]
    [SerializeField] private Camera _camera;

    [Header("Culling")]
    [Tooltip("カメラ外に広げる範囲(ビューポート比率。0.15=画面幅の15%ぶん外側まで表示)")]
    [SerializeField, Min(0f)] private float _offset = 0.15f;

    [Tooltip("ちらつき防止。表示中はOffset+この値を超えるまで非表示にしない")]
    [SerializeField, Min(0f)] private float _hysteresis = 0.05f;

    [Tooltip("1フレームに判定するEntity数(多いほど反応が早い/少ないほど軽い)")]
    [SerializeField, Min(1)] private int _checksPerFrame = 64;

    [Header("Indicator")]
    [Tooltip("画面外のEntityがどの方向にいるか表示するためのチェック")]
    [SerializeField] private bool _enableIndicator = true;

    [Tooltip("画面の縁からの余白")]
    [SerializeField] private float _edgePadding = 48f;

    [SerializeField] private float _iconSize = 40f;

    [Tooltip("未設定なら円を自動生成")]
    [SerializeField] private Sprite _iconSprite;

    [Tooltip("未設定なら三角形を自動生成(右向きの画像を設定すること)")]
    [SerializeField] private Sprite _arrowSprite;

    private NetTest_CullingProcessor _culling;
    private NetTest_OffscreenIndicatorView _indicatorView;

    private Camera _cachedCamera;
    private bool _warnedNoCamera;
    private bool _wasRunning;

    private void OnEnable()
    {
        _culling = new NetTest_CullingProcessor(_offset, _hysteresis); //カメラの描画範囲内設定クラスに情報をinject

        if (_enableIndicator)
        {
            _indicatorView = new NetTest_OffscreenIndicatorView(_iconSprite, _arrowSprite, _edgePadding, _iconSize); //画面外Entityに対してのicon表示用の情報を注入
        }
    }

    private void OnDisable()
    {
        _indicatorView?.Dispose();
        _indicatorView = null;
        _culling = null;
        _wasRunning = false;
    }

    // カメラ追従(LateUpdate)の後に判定したいのでLateUpdateで回す
    private void LateUpdate()
    {
        NetworkManager networkManager = NetworkManager.Singleton;
        bool isRunning = networkManager != null && networkManager.IsClient;
        if (!isRunning)
        {
            if (_wasRunning)
            {
                _indicatorView?.HideAll();
                _wasRunning = false;
            }

            return;
        }

        _wasRunning = true;

        Camera cam = ResolveCamera();

        if (cam == null) return;

        _culling.Tick(cam, NetTest_RenderRegistry.Targets, _checksPerFrame);
        _indicatorView?.Tick(cam, NetTest_RenderRegistry.IndicatorTargets);
    }

    private Camera ResolveCamera()
    {
        if (_camera != null) return _camera;

        //ヒエラルキー上のMainCameraがNullなら新規作成
        if (_cachedCamera == null) _cachedCamera = Camera.main;

        if (_cachedCamera == null && !_warnedNoCamera)
        {
            _warnedNoCamera = true;
            Debug.LogWarning("RenderManager : カメラが見つかりません。Cameraを設定するかMainCameraタグを付けてください", this);
        }

        return _cachedCamera;
    }
}