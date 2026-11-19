using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Unity.Netcode;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class NetTest_ServerDrivenObject : NetworkBehaviour
{
    private Enum_MovingMode _mode;
    private ulong _targetClientId; //ulong = unsigned long integer type.  unsigned longŒ^ / ulongŒ^
    private float _moveSpeed = 3f;
    private float _radius = 3f;
    private float _angle;
    private float _cycleTime = 2;
    private float _cycleTimer;
    private int _currentTargetIndex;

    public void Initialize(Enum_MovingMode mode, ulong targetClientId, float moveSpeed, float radius)
    {
        _mode = mode;
        _targetClientId = targetClientId;
        _moveSpeed = moveSpeed;
        _radius = radius;
    }

    private void Update()
    {
        // Host(Server)ˆÈŠO‚ÍˆÚ“®ˆ—‚ðŽÀs‚µ‚È‚¢
        if (!IsServer) return;

        switch (_mode)
        {
            case Enum_MovingMode.FollowHost:

                FollowPlayer(NetworkManager.ServerClientId);
                break;

            case Enum_MovingMode.FollowClients:

                FollowPlayer(_targetClientId);
                break;

            case Enum_MovingMode.Clockwise:

                MoveClockwise();
                break;

            case Enum_MovingMode.FollowInOrder:

                FollowInOrder();
                break;
        }
    }
    
    public void FollowPlayer(ulong clientId)
    {
        if (!NetworkManager.ConnectedClients.TryGetValue(clientId, out var client)) return;

        if (client.PlayerObject == null) return;

        Vector3 targetPosition = client.PlayerObject.transform.position;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, _moveSpeed * Time.deltaTime);
    }

    private void MoveClockwise()
    {
        _angle -= _moveSpeed * Time.deltaTime;

        float radian = _angle * Mathf.Deg2Rad;

        Vector3 position = new Vector3(Mathf.Cos(radian) * _radius, 0f, Mathf.Sin(radian) * _radius);

        transform.position = position;
    }

    private void FollowInOrder()
    {
        var client = NetworkManager.ConnectedClientsIds;
        if (client.Count == 0) return;
        _cycleTimer += Time.deltaTime;
        if(_cycleTimer >= _cycleTime)
        {
            _cycleTimer = 0;
            _currentTargetIndex++;
            if(_currentTargetIndex >= client.Count) _currentTargetIndex = 0;
        }
        ulong targetId = client[_currentTargetIndex];

        FollowPlayer(targetId);
    }
}
