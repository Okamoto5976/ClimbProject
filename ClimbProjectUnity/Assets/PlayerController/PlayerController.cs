using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference climbAction;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float climbSpeed = 3f;
    [SerializeField]private float downClimbSpeed=15f;
    
    private Rigidbody rb;
    private bool isClimbing=false;
    private bool canClimb=false;
    private ClimbStart currentClimbStart;
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
            currentClimbStart=other.GetComponent<ClimbStart>();
        }
        if (other.CompareTag("ClimbTop"))
        {
            isClimbing = false;
            rb.useGravity = true;
            rb.linearVelocity = Vector3.zero;

         }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("ClimbStart")&&!isClimbing)
        {
            canClimb = false;
            currentClimbStart = null;
        }
    }
    private void Update()
    {
        if (climbAction.action.triggered && canClimb)
        {
            isClimbing = true;

            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;

            float wallFront = currentClimbStart.WallCollider.bounds.min.z;

            Vector3 pos = transform.position;
            pos.z = wallFront - 1f;
            transform.position = pos;

            Debug.Log("•Ç‚Ì‚Ú‚è" + isClimbing);
        }
    }
    private void FixedUpdate()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();

        if (isClimbing)
        {
            float wallFront = currentClimbStart.WallCollider.bounds.min.z;

            Vector3 pos = transform.position;
            pos.z = wallFront - 1f;   
            transform.position = pos;

            float verticalSpeed = climbSpeed;

            if(input.y< 0&&Mathf.Abs(input.x)<0.1f)
            {
                verticalSpeed=downClimbSpeed;
            }
            rb.linearVelocity = new Vector3(
                input.x * climbSpeed,
                input.y * verticalSpeed,
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
