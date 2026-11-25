
using UnityEngine;

/// <summary>
/// Stageの見た目に関する設定データ
/// </summary>
[CreateAssetMenu(
    fileName = "StageData",
    menuName = "Game/Stage Data")]
public class StageData : ScriptableObject
{
    [Header("StageのMaterial")]
    [SerializeField] private Material[] _stageMaterials;

    /// <summary>
    /// 指定された段数に対応するMaterialを取得する
    /// </summary>
    public Material GetMaterial(int stage)
    {
        if (_stageMaterials == null || _stageMaterials.Length == 0)
        {
            return null;
        }

        int index = Mathf.Clamp(stage, 0, _stageMaterials.Length - 1);

        return _stageMaterials[index];
    }
}
