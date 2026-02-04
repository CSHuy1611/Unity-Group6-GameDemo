using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private GameManager gameManager;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;

    [Header("State")]
    public PlayerState currentState = PlayerState.Idle;

    [Header("Combat")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private LayerMask attackLayers;

    [Header("Ground Check")]
    [SerializeField] private bool isGrounded = true;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private bool inventoryOpen = false;
    private Vector2 moveDirection;
    private bool isAttacking = false;

    void Start()
    {
        InitializeComponents();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleInventory();
        }

        if (!inventoryOpen)
        {
            HandleMovementInput();
            HandleJumpInput();
            HandleAttackInput();
        }

        UpdateState();
        CheckGroundStatus();
    }

    void FixedUpdate()
    {
        if (!inventoryOpen)
        {
            ApplyMovement();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        HandleTriggerCollision(other);
    }

    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("PlayerController: Rigidbody component not found!");
        }

        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("PlayerController: GameManager not found in scene!");
        }

        currentState = PlayerState.Idle;
        Debug.Log("PlayerController initialized!");
    }

    private void HandleMovementInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        moveDirection = new Vector2(horizontal, 0f).normalized;
    }

    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    private void HandleAttackInput()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            StartAttack();
        }
    }

    private void ApplyMovement()
    {
        if (Mathf.Abs(moveDirection.x) > 0.1f)
        {
            rb.velocity = new Vector2(
                moveDirection.x * moveSpeed,
                rb.velocity.y
            );
        }
        else
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        currentState = PlayerState.Jump;
    }

    private void StartAttack()
    {
        isAttacking = true;
        currentState = PlayerState.Attack;
        Debug.Log("?? Player attacking!");

        CheckAttackTargets();

        StartCoroutine(ResetAttackState());
    }

    private void CheckAttackTargets()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, attackRange);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Tree"))
            {
                Tree tree = hitCollider.GetComponent<Tree>();
                if (tree != null)
                {
                    Debug.Log($"?? Hit tree: {hitCollider.name}");
                    tree.ChopDown();
                }
            }

            if (hitCollider.CompareTag("Chest"))
            {
                Chest chest = hitCollider.GetComponent<Chest>();
                if (chest != null)
                {
                    Debug.Log($"?? Hit chest: {hitCollider.name}");
                    chest.Collect();
                }
            }
        }
    }

    private IEnumerator ResetAttackState()
    {
        yield return new WaitForSeconds(0.3f);
        isAttacking = false;
        if (currentState == PlayerState.Attack)
        {
            currentState = PlayerState.Idle;
        }
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }

    private void ToggleInventory()
    {
        inventoryOpen = !inventoryOpen;

        if (inventoryOpen)
        {
            Debug.Log("=== INVENTORY OPENED ===");
            rb.velocity = Vector2.zero;

            if (gameManager != null)
            {
                gameManager.OpenInventory();
            }
        }
        else
        {
            Debug.Log("=== INVENTORY CLOSED ===");

            if (gameManager != null)
            {
                gameManager.CloseInventory();
            }
        }
    }

    private void HandleTriggerCollision(Collider2D other)
    {
        if (other.CompareTag("Chest"))
        {
            Debug.Log("?? Player touched chest!");
            Chest chest = other.GetComponent<Chest>();
            if (chest != null)
            {
                chest.Collect();
            }
        }
    }

    private void UpdateState()
    {
        if (currentState == PlayerState.Attack)
        {
            return;
        }

        if (!isGrounded)
        {
            currentState = PlayerState.Jump;
        }
        else if (moveDirection.magnitude > 0.1f)
        {
            currentState = PlayerState.Run;
        }
        else
        {
            currentState = PlayerState.Idle;
        }
    }

    private void CheckGroundStatus()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        isGrounded = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );
        if (Mathf.Abs(rb.velocity.y) < 0.1f)
        {
            isGrounded = true;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}