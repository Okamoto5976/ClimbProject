using UnityEngine;

/// <summary>
/// インジケーター用の簡易スプライトをコードで生成する(画像素材なしで動かすため)。
/// 生成したTextureは呼び出し側が破棄すること。
/// </summary>
public static class NetTest_IndicatorSprites
{
    /// <summary>
    /// 白い円スプライトを生成する。
    /// </summary>
    public static Sprite CreateCircle(int size = 64)
    {
        Texture2D texture = CreateTexture(size);
        Color32[] pixels = new Color32[size * size];

        float center = (size - 1) * 0.5f;
        float radius = size * 0.5f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - center;
                float dy = y - center;
                float distance = Mathf.Sqrt(dx * dx + dy * dy);

                // 1px幅でエッジをなだらかにする
                float alpha = Mathf.Clamp01(radius - distance);

                pixels[y * size + x] = new Color32(255, 255, 255, (byte)(alpha * 255f));
            }
        }

        return Finish(texture, pixels, size);
    }

    /// <summary>
    /// 右(+X)向きの白い三角形スプライトを生成する。
    /// </summary>
    public static Sprite CreateArrow(int size = 64)
    {
        Texture2D texture = CreateTexture(size);
        Color32[] pixels = new Color32[size * size];

        float center = (size - 1) * 0.5f;

        for (int y = 0; y < size; y++)
        {
            // 縦中央ほど右まで伸びる三角形
            float limitX = size * (1f - Mathf.Abs(y - center) / center);

            for (int x = 0; x < size; x++)
            {
                float alpha = Mathf.Clamp01(limitX - x);

                pixels[y * size + x] = new Color32(255, 255, 255, (byte)(alpha * 255f));
            }
        }

        return Finish(texture, pixels, size);
    }

    private static Texture2D CreateTexture(int size)
    {
        return new Texture2D(size, size, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };
    }

    private static Sprite Finish(Texture2D texture, Color32[] pixels, int size)
    {
        texture.SetPixels32(pixels);
        texture.Apply();

        return Sprite.Create(
            texture,
            new Rect(0, 0, size, size),
            new Vector2(0.5f, 0.5f),
            100f
        );
    }
}
