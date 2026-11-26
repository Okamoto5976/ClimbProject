using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private InputActionReference attackAction;
    [SerializeField] private GameObject attackPrefab;
    [SerializeField] private float attackDistance = 1f;
    [SerializeField] private float attackTime = 1f;
    [SerializeField] private float attackCooldown = 0.5f;

    private PlayerController playerController;
    private bool canAttack = true;
    private void OnEnable()
    {
        attackAction.action.Enable(); 
    }
    private void OnDisable()
    {
       attackAction.action.Disable();
    }
    private void Awake()
    {
        playerController=GetComponent<PlayerController>();
    }
    private void Update()
    {
        if (canAttack&&playerController.IsClimbing&&attackAction.action.triggered)
        {
            Attack();
        }
    }
    void Attack()
    {
        canAttack = false;

        Vector2 input =attackAction.action.ReadValue<Vector2>();

        if (Mathf.Abs(input.x) > 0)
        {
            input.y = 0;
        }

        if (input == Vector2.zero)
        {
            canAttack=true;
            return;
        }
        Vector3 direction = new Vector3(input.x, input.y, 0);

        Vector3 spawnPos = transform.position + direction * attackDistance;

        GameObject attack = Instantiate(attackPrefab, spawnPos, Quaternion.identity);

        Destroy(attack, attackTime);

        Invoke(nameof(ResetAttack), attackCooldown);
    }
  private void ResetAttack()
    {
        canAttack = true;
    }
}
