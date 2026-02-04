using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private GameManager gameManager;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;

    [Header("State")]
    public PlayerState currentState = PlayerState.Idle;

    [Header("Combat Settings")]
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private int attackDamage = 50;

    [Header("Ground Check")]
    [SerializeField] private bool isGrounded = true;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private bool inventoryOpen = false;
    
    private Vector3 moveDirection;
    
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
    
    void OnTriggerEnter(Collider other)
    {
        HandleTriggerCollision(other);
    }
    
    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody>();
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
        float vertical = Input.GetAxisRaw("Vertical");
        
        moveDirection = new Vector3(horizontal, 0f, vertical).normalized;
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
            Attack();
        }
    }
    
    private void ApplyMovement()
    {
        if (moveDirection.magnitude > 0.1f)
        {
            Vector3 movement = moveDirection * moveSpeed;
            rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
        }
        else
        {
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
        }
    }
    
    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        currentState = PlayerState.Jump;
        Debug.Log("Player jumped!");
    }
    
    private void Attack()
    {
        currentState = PlayerState.Attack;
        Debug.Log("Player attacking!");
        
        if (gameManager != null)
        {
            GameObject nearestEnemy = gameManager.FindNearestEnemy(transform.position, attackRange);
            
            if (nearestEnemy != null)
            {
                Enemy enemyScript = nearestEnemy.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    enemyScript.TakeDamage(attackDamage);
                    Debug.Log($"Hit enemy: {nearestEnemy.name} for {attackDamage} damage!");
                }
            }
            else
            {
                Debug.Log("No enemy in range!");
            }
        }
        
        StartCoroutine(ResetAttackState());
    }
    
    private IEnumerator ResetAttackState()
    {
        yield return new WaitForSeconds(0.5f);
        if (currentState == PlayerState.Attack)
        {
            currentState = PlayerState.Idle;
        }
    }
    
    private void ToggleInventory()
    {
        inventoryOpen = !inventoryOpen;
        
        if (inventoryOpen)
        {
            Debug.Log("=== INVENTORY OPENED ===");
            rb.velocity = Vector3.zero;
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
    
    public bool IsInventoryOpen()
    {
        return inventoryOpen;
    }

    private void HandleTriggerCollision(Collider other)
    {
        // Pickup Chest
        if (other.CompareTag("Chest"))
        {
            Debug.Log("Player collected chest!");
            Chest chest = other.GetComponent<Chest>();
            if (chest != null)
            {
                chest.Collect();
            }
        }
        
        // Pickup Treasure
        if (other.CompareTag("Treasure"))
        {
            Debug.Log("Player picked up treasure!");
            Treasure treasure = other.GetComponent<Treasure>();
            if (treasure != null)
            {
                treasure.Pickup();
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
        isGrounded = Physics.Raycast(ray, groundCheckDistance + 0.1f, groundLayer);
        
        if (Mathf.Abs(rb.velocity.y) < 0.1f)
        {
            isGrounded = true;
        }
    }
    
    public void TakeDamage(int damage)
    {
        Debug.Log($"Player took {damage} damage!");
    }
    
    public void Die()
    {
        currentState = PlayerState.Die;
        Debug.Log("Player died!");
    }
    
    // =============================================
    // DEBUG
    // =============================================
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, Vector3.down * (groundCheckDistance + 0.1f));
    }
}
