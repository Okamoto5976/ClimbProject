
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤー周辺のStageを生成・管理するクラス
/// </summary>
public class StageGenerator : MonoBehaviour
{
    [Header("プレイヤー設定")]
    [SerializeField] private Transform _player;

    [Header("Stage設定")]
    [SerializeField] private GameObject _stagePrefab;
    [SerializeField] private StageData _stageData;

    [Header("生成範囲")]
    [SerializeField, Min(0)] private int _upperCount = 2;
    [SerializeField, Min(0)] private int _lowerCount = 1;

    [Header("横方向の生成範囲")]
    [SerializeField, Min(0)] private int _leftCount = 1;
    [SerializeField, Min(0)] private int _rightCount = 2;

    [Header("生成間隔")]
    [SerializeField, Min(0.1f)] private float _horizontalSpacing = 20f;
    [SerializeField, Min(0.1f)] private float _verticalSpacing = 20f;
    [SerializeField] private float _depthOffset = 2f;

    [Header("最大生成段数")]
    [SerializeField, Min(1)] private int _maxStageCount = 20;

    private readonly Dictionary<Vector2Int, GameObject> _stageObjects = new();
    private readonly List<GameObject> _availableStages = new();

    private int _currentStage = -1;
    private int _currentColumn = int.MinValue;

    private void Start()
    {
        if (_player == null || _stagePrefab == null || _stageData == null)
        {
            Debug.LogError("StageGeneratorの設定が不足しています。", this);
            enabled = false;
            return;
        }

        UpdatePlayerPosition();
        RefreshStages();
    }

    private void Update()
    {
        UpdatePlayerPosition();

        if (_currentStage != _lastStage ||
            _currentColumn != _lastColumn)
        {
            RefreshStages();
        }
    }

    private int _lastStage = -1;
    private int _lastColumn = int.MinValue;

    /// <summary>
    /// プレイヤーの現在位置を取得する
    /// </summary>
    private void UpdatePlayerPosition()
    {
        _currentStage = Mathf.Clamp(
            Mathf.FloorToInt(_player.position.y / _verticalSpacing),
            0,
            _maxStageCount - 1
        );

        // 横方向の間隔を半分にする
        float horizontalStep = _horizontalSpacing / 2f;

        // 最も近い列をプレイヤーの中央にする
        _currentColumn = Mathf.RoundToInt(
            _player.position.x / horizontalStep);
    }

    /// <summary>
    /// プレイヤー周辺のStageを更新する
    /// </summary>
    private void RefreshStages()
    {
        _lastStage = _currentStage;
        _lastColumn = _currentColumn;

        HashSet<Vector2Int> requiredPositions = new();

        int startColumn = _currentColumn - _leftCount;
        int endColumn = _currentColumn + _rightCount;

        int startStage = Mathf.Max(0, _currentStage - _lowerCount);

        int endStage = Mathf.Min(
            _currentStage + _upperCount,
            _maxStageCount - 1
        );

        // 必要な配置位置を登録
        for (int stage = startStage; stage <= endStage; stage++)
        {
            for (int column = startColumn; column <= endColumn; column++)
            {
                requiredPositions.Add(new Vector2Int(column, stage));
            }
        }

        // 不要なPrefabを再利用リストへ移動
        List<Vector2Int> removeKeys = new();

        foreach (var pair in _stageObjects)
        {
            if (!requiredPositions.Contains(pair.Key))
            {
                removeKeys.Add(pair.Key);
            }
        }

        foreach (Vector2Int key in removeKeys)
        {
            GameObject stageObject = _stageObjects[key];

            _stageObjects.Remove(key);
            _availableStages.Add(stageObject);
        }

        // 進行方向の右側を先に生成する
        for (int stage = startStage; stage <= endStage; stage++)
        {
            for (int column = _currentColumn + _rightCount;
                 column >= _currentColumn - _leftCount;
                 column--)
            {
                GenerateStage(column, stage);
            }
        }
    }

    /// <summary>
    /// 指定位置にStageを配置する
    /// </summary>
    private void GenerateStage(int column, int stage)
    {
        Vector2Int key = new Vector2Int(column, stage);

        if (_stageObjects.ContainsKey(key))
        {
            return;
        }

        GameObject stageObject;

        if (_availableStages.Count > 0)
        {
            int index = _availableStages.Count - 1;
            stageObject = _availableStages[index];
            _availableStages.RemoveAt(index);
        }
        else
        {
            stageObject = Instantiate(_stagePrefab, transform);
        }

        float horizontalStep = _horizontalSpacing / 2f;

        // ワールド座標を基準に固定配置
        Vector3 position = new Vector3(
            column * horizontalStep,
            stage * _verticalSpacing,
            stage * _depthOffset
        );

        stageObject.transform.position = position;
        stageObject.transform.rotation = Quaternion.identity;
        stageObject.SetActive(true);

        ApplyStageMaterial(stageObject, stage);

        _stageObjects.Add(key, stageObject);
    }


    /// <summary>
    /// 段数に応じたMaterialを適用する
    /// </summary>
    private void ApplyStageMaterial(GameObject stageObject, int stage)
    {
        Material material = _stageData.GetMaterial(stage);

        if (material == null)
        {
            Debug.LogWarning(
                $"段数 {stage} のMaterialが設定されていません。",
                this);
            return;
        }

        Renderer[] renderers =
            stageObject.GetComponentsInChildren<Renderer>(true);

        foreach (Renderer renderer in renderers)
        {
            renderer.sharedMaterial = material;
        }
    }
}
