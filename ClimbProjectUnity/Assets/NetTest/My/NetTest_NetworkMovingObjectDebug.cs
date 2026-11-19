using Unity.Netcode;
using UnityEngine;

public class NetworkMovingObjectDebug : MonoBehaviour
{
    [SerializeField] private NetTest_NetworkMovingObjectManager _manager;

    private float _moveSpeed = 3f;
    private float _radius = 3f;

    private void OnGUI()
    {
        var networkManager = NetworkManager.Singleton;

        if (networkManager == null) return;
        if (!networkManager.IsHost) return;

        GUILayout.BeginArea(new Rect(370, 10, 300, 450), GUI.skin.box);
        GUILayout.Label("Moving Object Debug");
        GUILayout.Space(10);

        if (GUILayout.Button("Host Follow"))
        {
            _manager.SetMode(Enum_MovingMode.FollowHost);
        }

        if (GUILayout.Button("Client Follow"))
        {
            _manager.SetMode(Enum_MovingMode.FollowClients);
        }

        if (GUILayout.Button("Clockwise"))
        {
            _manager.SetMode(Enum_MovingMode.Clockwise);
        }

        if (GUILayout.Button("Follow In Order"))
        {
            _manager.SetMode(Enum_MovingMode.FollowInOrder);
        }

        GUILayout.Space(20);
        GUILayout.Label($"Move Speed : {_moveSpeed:F1}");
        float speed = GUILayout.HorizontalSlider(_moveSpeed,0.5f, 15f);
        if (!Mathf.Approximately(speed, _moveSpeed))
        {
            _moveSpeed = speed;
            _manager.SetMoveSpeed(_moveSpeed);
        }

        GUILayout.Space(10);
        GUILayout.Label($"Radius : {_radius:F1}");
        float radius = GUILayout.HorizontalSlider(_radius, 1f, 15f);
        if (!Mathf.Approximately(radius, _radius))
        {
            _radius = radius;
            _manager.SetRadius(_radius);
        }

        GUILayout.Space(10);
        GUILayout.Label($"Connected Players : " + networkManager.ConnectedClientsIds.Count);
        GUILayout.EndArea();
    }
}