using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Host用のデバッグUI。
/// </summary>
public class NetTest_NetworkMovingObjectDebug : MonoBehaviour
{
    [SerializeField]
    private NetTest_NetworkMovingObjectManager _manager;

#if UNITY_EDITOR || DEVELOPMENT_BUILD

    private static readonly Rect AreaRect =
        new Rect(370, 10, 300, 450);

    private bool _isInitialized;

    private float _moveSpeed;
    private float _moveInterval;

    private int _playerCount = -1;

    private string _speedLabel;
    private string _intervalLabel;
    private string _playerCountLabel;

    private void OnGUI()
    {
        NetworkManager networkManager =
            NetworkManager.Singleton;

        if (networkManager == null ||
            !networkManager.IsHost)
            return;

        if (_manager == null) return;

        if (!_isInitialized)
        {
            Initialize();
        }

        GUILayout.BeginArea(AreaRect, GUI.skin.box);

        GUILayout.Label("Moving Object Debug");

        GUILayout.Space(10);

        DrawModeButton(
            "Host Follow",
            Enum_MovingMode.FollowHost
        );

        DrawModeButton(
            "Client Follow",
            Enum_MovingMode.FollowClients
        );

        DrawModeButton(
            "Random Move",
            Enum_MovingMode.RandomMove
        );

        GUILayout.Space(20);

        // 移動速度
        GUILayout.Label(_speedLabel);

        float speed =
            GUILayout.HorizontalSlider(
                _moveSpeed,
                0.5f,
                15f
            );

        if (!Mathf.Approximately(speed, _moveSpeed))
        {
            _moveSpeed = speed;

            _speedLabel =
                $"Move Speed : {_moveSpeed:F1}";

            _manager.SetMoveSpeed(_moveSpeed);
        }

        GUILayout.Space(10);

        // 移動方向の変更間隔
        GUILayout.Label(_intervalLabel);

        float interval =
            GUILayout.HorizontalSlider(
                _moveInterval,
                0.5f,
                10f
            );

        if (!Mathf.Approximately(interval, _moveInterval))
        {
            _moveInterval = interval;

            _intervalLabel =
                $"Move Interval : {_moveInterval:F1} sec";

            _manager.SetMoveInterval(_moveInterval);
        }

        GUILayout.Space(10);

        // 接続人数
        int count =
            networkManager.ConnectedClientsIds.Count;

        if (count != _playerCount)
        {
            _playerCount = count;

            _playerCountLabel =
                $"Connected Players : {count}";
        }

        GUILayout.Label(_playerCountLabel);

        GUILayout.EndArea();
    }

    private void Initialize()
    {
        _isInitialized = true;

        _moveSpeed = _manager.MoveSpeed;

        _moveInterval = _manager.MoveInterval;

        _speedLabel =
            $"Move Speed : {_moveSpeed:F1}";

        _intervalLabel =
            $"Move Interval : {_moveInterval:F1} sec";
    }

    private void DrawModeButton(
        string label,
        Enum_MovingMode mode)
    {
        if (GUILayout.Button(label))
        {
            _manager.SetMode(mode);
        }
    }

#endif
}