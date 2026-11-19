using UnityEngine;

public class ClimbStart : MonoBehaviour
{
    [SerializeField] private BoxCollider wallCollider;

    public BoxCollider WallCollider => wallCollider;
}