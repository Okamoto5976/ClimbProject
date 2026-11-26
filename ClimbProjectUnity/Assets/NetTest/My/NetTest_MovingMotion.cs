using Unity.Netcode;
using UnityEngine;

/// <summary>
/// 敵1体分の移動処理を担当するクラス。
/// Managerから毎フレームTickされる。
/// </summary>
public sealed class NetTest_MovingMotion
{
    private readonly NetworkManager _networkManager;
    private readonly NetworkObject _networkObject;
    private readonly Transform _transform;

    private Enum_MovingMode _mode;

    private ulong _targetClientId;

    private float _moveSpeed;
    private float _moveInterval;

    private float _moveTimer;

    private Vector2 _moveDirection;

    // 追従対象のTransformをキャッシュ
    private Transform _cachedTarget;
    private ulong _cachedTargetId;

    public NetworkObject NetworkObject => _networkObject;

    public NetTest_MovingMotion(
        NetworkManager networkManager,
        NetworkObject networkObject)
    {
        _networkManager = networkManager;
        _networkObject = networkObject;
        _transform = networkObject.transform;
    }

    /// <summary>
    /// 移動モードと各種設定を初期化する
    /// </summary>
    public void Setup(
        Enum_MovingMode mode,
        ulong targetClientId,
        float moveSpeed,
        float moveInterval)
    {
        _mode = mode;
        _targetClientId = targetClientId;

        _moveSpeed = moveSpeed;
        _moveInterval = moveInterval;

        _moveTimer = 0f;

        _cachedTarget = null;

        if (_mode == Enum_MovingMode.RandomMove)
        {
            ChangeDirection();
        }
    }

    /// <summary>
    /// 移動速度を変更
    /// </summary>
    public void SetMoveSpeed(float value)
    {
        _moveSpeed = value;
    }

    /// <summary>
    /// 移動方向の変更間隔を変更
    /// </summary>
    public void SetMoveInterval(float value)
    {
        _moveInterval = Mathf.Max(0.01f, value);
    }

    /// <summary>
    /// 1フレーム分の移動処理
    /// </summary>
    public void Tick(float deltaTime)
    {
        switch (_mode)
        {
            case Enum_MovingMode.FollowHost:
            case Enum_MovingMode.FollowClients:

                FollowPlayer(_targetClientId, deltaTime);
                break;

            case Enum_MovingMode.RandomMove:

                RandomMove(deltaTime);
                break;
        }
    }

    /// <summary>
    /// 指定したPlayerを追従する
    /// </summary>
    private void FollowPlayer(
        ulong clientId,
        float deltaTime)
    {
        Transform target = ResolveTarget(clientId);

        if (target == null) return;

        _transform.position = Vector3.MoveTowards(
            _transform.position,
            target.position,
            _moveSpeed * deltaTime
        );
    }

    /// <summary>
    /// 追従対象のPlayerを取得
    /// </summary>
    private Transform ResolveTarget(ulong clientId)
    {
        if (_cachedTarget != null &&
            _cachedTargetId == clientId)
        {
            return _cachedTarget;
        }

        NetworkObject player =
            _networkManager.SpawnManager
            .GetPlayerNetworkObject(clientId);

        _cachedTarget = player != null
            ? player.transform
            : null;

        _cachedTargetId = clientId;

        return _cachedTarget;
    }

    /// <summary>
    /// 一定時間ごとにランダム方向へ移動する
    /// </summary>
    private void RandomMove(float deltaTime)
    {
        _moveTimer += deltaTime;

        if (_moveTimer >= _moveInterval)
        {
            _moveTimer = 0f;

            ChangeDirection();
        }

        Vector3 move = new Vector3(
            _moveDirection.x,
            _moveDirection.y,
            0f
        );

        _transform.position +=
            move * (_moveSpeed * deltaTime);
    }

    /// <summary>
    /// 45 / 135 / 225 / 315度から方向を選択
    /// </summary>
    private void ChangeDirection()
    {
        int[] angles = { 45, 135, 225, 315 };

        int randomIndex =
            Random.Range(0, angles.Length);

        float radian =
            angles[randomIndex] * Mathf.Deg2Rad;

        _moveDirection = new Vector2(
            Mathf.Cos(radian),
            Mathf.Sin(radian)
        );

        Debug.Log(
            $"Enemy Direction : {angles[randomIndex]}"
        );
    }
}