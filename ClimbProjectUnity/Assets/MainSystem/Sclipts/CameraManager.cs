using UnityEngine;

public class CameraManager : MonoBehaviour
{
    //Initialize‚©‚çplayerÀ•W‚ğ‚à‚Á‚Ä‚­‚é
    private Vector3 _playerPos;


    [SerializeField] private GameObject _player;//test

    private void Update()
    {
        _playerPos = _player.transform.position;//test


        _playerPos.z -= 10;
        this.gameObject.transform.position = _playerPos;
    }
}
