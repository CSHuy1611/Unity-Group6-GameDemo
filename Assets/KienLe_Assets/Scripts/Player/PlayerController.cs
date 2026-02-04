using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private GameManager gameManager;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Combat")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private LayerMask attackLayers;
    [SerializeField] private float attackCooldown = 0.3f;

    public PlayerState currentState = PlayerState.Idle;

    private float moveInput;
    private bool isGrounded;
    private bool isAttacking;
    private bool inventoryOpen;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        gameManager = FindObjectOfType<GameManager>();

        rb.freezeRotation = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleInventory();
        }

        if (inventoryOpen) return;

        ReadInput();
        UpdateState();
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        if (inventoryOpen) return;

        CheckGround();
        Move();
    }

    private void ReadInput()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isAttacking)
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.Return) && !isAttacking)
        {
            StartCoroutine(Attack());
        }
    }

    private void Move()
    {
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        if (moveInput != 0)
        {
            transform.localScale = new Vector3(
                Mathf.Sign(moveInput),
                1,
                1
            );
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheckPoint.position,
            groundCheckRadius,
            groundLayer
        );
    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        currentState = PlayerState.Attack;

        animator.SetTrigger("Attack");

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            attackRange,
            attackLayers
        );

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Tree"))
            {
                hit.GetComponent<Tree>()?.ChopDown();
            }
            else if (hit.CompareTag("Chest"))
            {
                hit.GetComponent<Chest>()?.Collect();
            }
        }

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    private void UpdateState()
    {
        if (isAttacking) return;

        if (!isGrounded)
        {
            currentState = PlayerState.Jump;
        }
        else if (Mathf.Abs(moveInput) > 0.1f)
        {
            currentState = PlayerState.Run;
        }
        else
        {
            currentState = PlayerState.Idle;
        }
    }

    private void UpdateAnimation()
    {
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("Speed", Mathf.Abs(moveInput));
        animator.SetInteger("State", (int)currentState);
    }

    private void ToggleInventory()
    {
        inventoryOpen = !inventoryOpen;

        if (inventoryOpen)
        {
            rb.velocity = Vector2.zero;
            gameManager?.OpenInventory();
        }
        else
        {
            gameManager?.CloseInventory();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        }
    }
}
