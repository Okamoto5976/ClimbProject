using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float climbSpeed = 3f;
    
    private Rigidbody rb;
    private bool isClimbing=false;
    void Awake()
    {
        rb=GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        moveAction.action.Enable();
    }
    private void OnDisable()
    {
        moveAction.action.Disable();
    }
    private void FixedUpdate()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();

        if (isClimbing)
        {
            rb.linearVelocity = new Vector3(
                input.x * climbSpeed,
                input.y * climbSpeed,
                0
            );
        }
        else
        {
            rb.linearVelocity = new Vector3(
                input.x * moveSpeed,
                rb.linearVelocity.y,
                0
            );
        }
    }
}
