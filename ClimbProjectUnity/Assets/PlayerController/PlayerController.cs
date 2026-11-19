using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference climbAction;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float climbSpeed = 3f;
    
    private Rigidbody rb;
    private bool isClimbing=false;
    private bool canClimb=false;
    void Awake()
    {
        rb=GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        moveAction.action.Enable();
        climbAction.action.Enable();
    }
    private void OnDisable()
    {
        moveAction.action.Disable();
        climbAction.action.Disable();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ClimbStart"))
        {
            canClimb = true;
        }
        if (other.CompareTag("ClimbTop"))
        {
            isClimbing = false;
            rb.useGravity = true;

            transform.position += transform.forward + Vector3.up;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("ClimbStart"))
        {
            canClimb = false;
        }
    }

    private void FixedUpdate()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();

        if(climbAction.action.triggered&&canClimb)
        {
            isClimbing = !isClimbing;
            rb.useGravity = !isClimbing;
            Debug.Log("•Ç‚Ì‚Ú‚è" + isClimbing);
        }
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
