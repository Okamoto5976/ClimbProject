using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference climbAction;

    [Header("Move")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float climbSpeed = 10f;
    [SerializeField] private float downClimbSpeed = 15f;

    [Header("Wall")]
    [SerializeField] private float wallCheckDistance = 1.5f;
    [SerializeField] private float wallOffset = 1f;
    [SerializeField] private LayerMask wallLayer;

    [Header("Climb Over")]
    [SerializeField] private float climbUpDistance = 2f;
    [SerializeField] private float climbForwardOffset = 1.5f;

    [Header("Ray Height")]
    [SerializeField] private float rayHeight = 1.5f;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerAttack playerAttack;

    private Rigidbody rb;
    private bool isClimbing = false;
    public bool IsClimbing => isClimbing;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
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

    private void Update()
    {
        CheckClimbStart();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }
    private void CheckClimbStart()
    {
        if (climbAction.action.triggered)
        {
            Vector3 origin = transform.position + Vector3.up * rayHeight;

            if (!isClimbing &&
                Physics.Raycast(origin, transform.forward, out RaycastHit hit, wallCheckDistance, wallLayer))
            {
                StartClimb(hit);
            }
        }
    }
    private void MovePlayer()
    {
        Vector3 origin = transform.position + Vector3.up * rayHeight;


        bool hitWall = Physics.Raycast(
            origin,
            transform.forward,
            out RaycastHit hit,
            wallCheckDistance,
            wallLayer);

        DrawWallRay(origin);

        if (isClimbing&&!hitWall)
        {
            StartClimbOver();
            return;
        }

        if (isClimbing)
        {
            ClimbMove(hit);
        }
        else
        {
            GroundMove();
        }
    }
    private void ClimbMove(RaycastHit hit)
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();

        Vector3 pos = transform.position;
        pos.z = hit.point.z - wallOffset;
        transform.position = pos;

        float verticalSpeed = climbSpeed;

        if (input.y < 0 && Mathf.Abs(input.x) < 0.1f)
        {
            verticalSpeed = downClimbSpeed;
        }

        rb.linearVelocity = new Vector3(
            input.x * climbSpeed,
            input.y * verticalSpeed,
            0);
    }
    private void GroundMove()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        
        rb.linearVelocity = new Vector3(
            input.x * moveSpeed,
            rb.linearVelocity.y,
            0);
    }
    private void DrawWallRay(Vector3 origin)
    {
        Color color = Physics.Raycast(
            origin,
            transform.forward,
            wallCheckDistance,
            wallLayer)
            ? Color.green
            : Color.red;

        Debug.DrawRay(
            origin,
            transform.forward * wallCheckDistance,
            color);
    }
    private void StartClimb(RaycastHit hit)
    {
        isClimbing = true;

        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;

        Vector3 pos = transform.position;
        pos.z = hit.point.z - wallOffset;
        transform.position = pos;
    }

    private void StartClimbOver()
    {
        rb.linearVelocity = Vector3.zero;

        FinishClimb();

        // animator.SetTrigger("Animator");
    }
    private void FinishClimb()
    {

        isClimbing = false;

        rb.useGravity = true;

        Vector3 pos = transform.position;
        pos.y += climbUpDistance;
        pos.z += climbForwardOffset;

        transform.position = pos;
    }
}