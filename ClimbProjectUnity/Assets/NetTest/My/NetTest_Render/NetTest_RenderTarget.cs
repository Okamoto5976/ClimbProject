using System;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// カリング対象(Player / 敵 / アイテム)のPrefabルートに付けるコンポーネント。
/// ・カメラ外に出たとき、Rendererと指定Behaviourを無効化する(そのPCだけの処理)
/// ・ShowIndicatorがONなら、画面外にいるときに方向矢印を表示する対象になる
/// Awakeは使わず、OnNetworkSpawn / OnNetworkDespawnで登録・解除する。
/// </summary>
public class NetTest_RenderTarget : NetworkBehaviour
{
    [Header("Culling")]
    [Tooltip("カメラ外のとき止めたいBehaviour(Animatorや見た目用スクリプトなど)。\n" +
             "NetworkBehaviour / NetworkTransformは同期が崩れるので入れないこと。")]
    [SerializeField] private Behaviour[] _pauseBehaviours;

    [Header("Indicator")]
    [Tooltip("画面外にいるとき、方向インジケーターを出す(Player向け)")]
    [SerializeField]
    private bool _showIndicator;

    [Tooltip("ONならOwnerのClientIdから色を自動決定する")]
    [SerializeField]
    private bool _useOwnerColor = true;

    [SerializeField]
    private Color _indicatorColor = Color.white;

    private Renderer[] _renderers;
    private bool[] _rendererDefaults;
    private bool[] _behaviourDefaults;

    private bool _isCulled;

    /// <summary>カリング状態が変化したときに通知される(true=非表示)</summary>
    public event Action<bool> onCulledChanged;

    public bool IsCulled => _isCulled;

    public bool ShowIndicator => _showIndicator;

    public Color IndicatorColor =>
        _useOwnerColor
            ? NetTest_PlayerColor.Get(OwnerClientId)
            : _indicatorColor;

    public override void OnNetworkSpawn()
    {
        CacheComponents();

        _isCulled = false;

        NetTest_RenderRegistry.Register(this);
    }

    public override void OnNetworkDespawn()
    {
        NetTest_RenderRegistry.Unregister(this);

        // プール再利用などに備えて元の状態に戻す
        SetCulled(false);
    }

    /// <summary>
    /// カリング状態を切り替える。
    /// true : RendererとpauseBehavioursを無効化 / false : 元の状態に戻す
    /// </summary>
    public void SetCulled(bool isCulled)
    {
        if (_isCulled == isCulled) return;

        _isCulled = isCulled;

        if (_renderers != null)
        {
            for (int i = 0; i < _renderers.Length; i++)
            {
                if (_renderers[i] == null) continue;

                _renderers[i].enabled = !isCulled && _rendererDefaults[i];
            }
        }

        if (_pauseBehaviours != null)
        {
            for (int i = 0; i < _pauseBehaviours.Length; i++)
            {
                if (_pauseBehaviours[i] == null) continue;

                _pauseBehaviours[i].enabled = !isCulled && _behaviourDefaults[i];
            }
        }

        onCulledChanged?.Invoke(isCulled);
    }

    /// <summary>
    /// 子を含むRendererと、元のenabled状態をキャッシュする
    /// </summary>
    private void CacheComponents()
    {
        _renderers = GetComponentsInChildren<Renderer>(true);
        _rendererDefaults = new bool[_renderers.Length];

        for (int i = 0; i < _renderers.Length; i++)
        {
            _rendererDefaults[i] = _renderers[i].enabled;
        }

        int behaviourCount = _pauseBehaviours != null ? _pauseBehaviours.Length : 0;
        _behaviourDefaults = new bool[behaviourCount];

        for (int i = 0; i < behaviourCount; i++)
        {
            _behaviourDefaults[i] = _pauseBehaviours[i] != null && _pauseBehaviours[i].enabled;
        }
    }
}
