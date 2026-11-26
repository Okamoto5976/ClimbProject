using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 画面外にいる仲間の方向を、画面の縁に「色付きアイコン＋矢印」で表示するビュー。
/// Canvasはコードで生成するので、シーンへのUI配置は不要。
/// 位置は正規化アンカーで指定するため、解像度が変わってもズレない。
/// </summary>
public sealed class NetTest_OffscreenIndicatorView
{
    private sealed class Item
    {
        public RectTransform Root;
        public RectTransform ArrowPivot;
        public Image Icon;
        public Image Arrow;
        public Color Color;
    }

    private static readonly Vector2 Half = new Vector2(0.5f, 0.5f);

    private readonly List<Item> _items = new();

    private readonly GameObject _canvasObject;
    private readonly Canvas _canvas;
    private readonly RectTransform _canvasRect;

    private readonly Sprite _iconSprite;
    private readonly Sprite _arrowSprite;
    private readonly bool _ownsIcon;
    private readonly bool _ownsArrow;

    private readonly float _edgePadding;
    private readonly float _iconSize;
    private readonly float _arrowSize;

    /// <param name="iconSprite">nullなら円を自動生成</param>
    /// <param name="arrowSprite">nullなら三角形を自動生成(右向きの画像を渡すこと)</param>
    /// <param name="edgePadding">画面の縁からの余白(キャンバス単位)</param>
    /// <param name="iconSize">アイコンの大きさ(キャンバス単位)</param>
    public NetTest_OffscreenIndicatorView(
        Sprite iconSprite,
        Sprite arrowSprite,
        float edgePadding,
        float iconSize)
    {
        _ownsIcon = iconSprite == null;
        _ownsArrow = arrowSprite == null;

        _iconSprite = _ownsIcon ? NetTest_IndicatorSprites.CreateCircle() : iconSprite;
        _arrowSprite = _ownsArrow ? NetTest_IndicatorSprites.CreateArrow() : arrowSprite;

        _edgePadding = edgePadding;
        _iconSize = iconSize;
        _arrowSize = iconSize * 0.5f;

        _canvasObject = new GameObject("NetTest_IndicatorCanvas");

        _canvas = _canvasObject.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 100;

        CanvasScaler scaler = _canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        _canvasRect = _canvasObject.GetComponent<RectTransform>();
    }

    /// <summary>
    /// 毎フレーム呼ぶ。画面外のターゲットぶんだけインジケーターを配置し、余りは非表示にする。
    /// </summary>
    public void Tick(Camera camera, IReadOnlyList<NetTest_RenderTarget> targets)
    {
        if (Screen.width <= 0 || Screen.height <= 0)
        {
            HideAll();
            return;
        }

        // 縁の余白を画面比率(0〜1)に変換
        float padPixels = _edgePadding * _canvas.scaleFactor;
        float halfX = 0.5f - padPixels / Screen.width;
        float halfY = 0.5f - padPixels / Screen.height;

        int used = 0;

        for (int i = 0; i < targets.Count; i++)
        {
            NetTest_RenderTarget target = targets[i];

            if (target == null || target.IsLocalPlayer) continue;

            if (!TryGetEdgePoint(camera, target.transform.position, halfX, halfY,
                    out Vector2 anchor, out float angle))
            {
                continue;
            }

            Item item = GetOrCreate(used);
            used++;

            Apply(item, anchor, angle, target.IndicatorColor);
        }

        for (int i = used; i < _items.Count; i++)
        {
            SetActive(_items[i], false);
        }
    }

    /// <summary>
    /// インジケーターを全て非表示にする。
    /// </summary>
    public void HideAll()
    {
        for (int i = 0; i < _items.Count; i++)
        {
            SetActive(_items[i], false);
        }
    }

    /// <summary>
    /// 生成したCanvasとスプライトを破棄する。
    /// </summary>
    public void Dispose()
    {
        if (_canvasObject != null)
        {
            Object.Destroy(_canvasObject);
        }

        DestroyGenerated(_iconSprite, _ownsIcon);
        DestroyGenerated(_arrowSprite, _ownsArrow);

        _items.Clear();
    }

    /// <summary>
    /// ワールド座標が画面外なら、画面の縁の位置(ビューポート比率)と矢印角度を返す。
    /// 画面内ならfalse。
    /// </summary>
    private static bool TryGetEdgePoint(Camera camera, Vector3 worldPosition, float halfX, float halfY, out Vector2 anchor, out float angle) 
    {
        anchor = Half;
        angle = 0f;

        Vector3 viewport = camera.WorldToViewportPoint(worldPosition);

        Vector2 d = new Vector2(viewport.x - 0.5f, viewport.y - 0.5f);

        // カメラの後ろにある場合は方向が反転して見えるので戻す
        if (viewport.z < 0f)
        {
            d = -d;
        }

        float ax = Mathf.Abs(d.x);
        float ay = Mathf.Abs(d.y);

        // 画面内(余白の内側)なら表示しない
        if (viewport.z > 0f && ax <= halfX && ay <= halfY) return false;

        // 中心とほぼ同じ位置(真後ろなど)は方向が決まらない
        if (ax < 0.0001f && ay < 0.0001f) return false;

        // 中心→ターゲット方向のレイが、余白つき矩形の縁に当たる倍率
        float tx = ax > 0.0001f ? halfX / ax : float.MaxValue;
        float ty = ay > 0.0001f ? halfY / ay : float.MaxValue;
        float t = Mathf.Min(tx, ty);

        anchor = new Vector2(0.5f + d.x * t, 0.5f + d.y * t);

        // 縦横比を考慮したピクセル方向で角度を出す
        angle = Mathf.Atan2(d.y * Screen.height, d.x * Screen.width) * Mathf.Rad2Deg;

        return true;
    }

    private void Apply(Item item, Vector2 anchor, float angle, Color color)
    {
        SetActive(item, true);

        item.Root.anchorMin = anchor;
        item.Root.anchorMax = anchor;
        item.Root.anchoredPosition = Vector2.zero;

        item.ArrowPivot.localRotation = Quaternion.Euler(0f, 0f, angle);

        if (item.Color != color)
        {
            item.Color = color;
            item.Icon.color = color;
            item.Arrow.color = color;
        }
    }

    private static void SetActive(Item item, bool isActive)
    {
        if (item.Root.gameObject.activeSelf != isActive)
        {
            item.Root.gameObject.SetActive(isActive);
        }
    }

    private Item GetOrCreate(int index)
    {
        while (_items.Count <= index)
        {
            _items.Add(CreateItem());
        }

        return _items[index];
    }

    /// <summary>
    /// Root(アイコン) └ ArrowPivot(回転軸) └ Arrow の階層を作る。
    /// ArrowPivotを回すと、矢印がアイコンの周りを回る。
    /// </summary>
    private Item CreateItem()
    {
        // Root
        GameObject rootObject = new GameObject("Indicator", typeof(RectTransform));
        RectTransform root = rootObject.GetComponent<RectTransform>();
        root.SetParent(_canvasRect, false);
        root.pivot = Half;
        root.sizeDelta = new Vector2(_iconSize, _iconSize);

        Image icon = rootObject.AddComponent<Image>();
        icon.sprite = _iconSprite;
        icon.raycastTarget = false;

        // ArrowPivot(アイコン中心に置く回転軸)
        GameObject pivotObject = new GameObject("ArrowPivot", typeof(RectTransform));
        RectTransform pivot = pivotObject.GetComponent<RectTransform>();
        pivot.SetParent(root, false);
        pivot.anchorMin = Half;
        pivot.anchorMax = Half;
        pivot.pivot = Half;
        pivot.sizeDelta = Vector2.zero;
        pivot.anchoredPosition = Vector2.zero;

        // Arrow(アイコンの外側に出す。画像は右向き前提)
        GameObject arrowObject = new GameObject("Arrow", typeof(RectTransform));
        RectTransform arrowRect = arrowObject.GetComponent<RectTransform>();
        arrowRect.SetParent(pivot, false);
        arrowRect.anchorMin = Half;
        arrowRect.anchorMax = Half;
        arrowRect.pivot = new Vector2(0f, 0.5f);
        arrowRect.sizeDelta = new Vector2(_arrowSize, _arrowSize);
        arrowRect.anchoredPosition = new Vector2(_iconSize * 0.5f + 2f, 0f);

        Image arrow = arrowObject.AddComponent<Image>();
        arrow.sprite = _arrowSprite;
        arrow.raycastTarget = false;

        return new Item
        {
            Root = root,
            ArrowPivot = pivot,
            Icon = icon,
            Arrow = arrow,
            Color = Color.clear
        };
    }

    private static void DestroyGenerated(Sprite sprite, bool owns)
    {
        if (!owns || sprite == null) return;

        Object.Destroy(sprite.texture);
        Object.Destroy(sprite);
    }
}
