using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] Animator animator;

    [Header("Movement Settings")]
    public float moveSpeed = 7f;
    public float jumpForce = 14f;
    public float dashForce = 20f;
    public float dashDuration = 0.2f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private PlayerInputActions inputActions;

    private Vector2 moveInput;
    private bool isGrounded;
    private bool canDoubleJump;
    private bool isDashing;
    private float dashTimeLeft;
    private bool facingRight = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputActions = new PlayerInputActions();

        inputActions.Player.Jump.performed += ctx => OnJump();
        inputActions.Player.Dash.performed += ctx => OnDash();
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();

    private void Update()
    {
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        CheckGround();
        HandleDashTimer();
        FlipSprite();
    }

    private void FixedUpdate()
    {
        if (isDashing) return; // 대시 중에는 이동 입력 무시

        // float targetVelocityX = moveInput.x * moveSpeed;
        // float smoothX = Mathf.Lerp(rb.linearVelocity.x, targetVelocityX, 0.2f);
        // rb.linearVelocity = new Vector2(smoothX, rb.linearVelocity.y);

        float moveX = moveInput.x * moveSpeed;
        animator.SetBool("IsMoving", moveX != 0);
        rb.linearVelocity = new Vector2(moveX, rb.linearVelocity.y);
    }

    private void OnJump()
    {
        if (isGrounded)
        {
            Jump();
            canDoubleJump = true;
        }
        else if (canDoubleJump)
        {
            Jump();
            canDoubleJump = false;
        }
    }

    private void Jump()
    {
        // 수직 속도 초기화 후 점프력 적용
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void OnDash()
    {
        if (isDashing) return;

        isDashing = true;
        dashTimeLeft = dashDuration;
        animator.CrossFade("PlayerSkill", 0);

        float dashDir = facingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dashDir * dashForce, 0f);
    }

    private void HandleDashTimer()
    {
        if (isDashing)
        {
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0f)
            {
                isDashing = false;
            }
        }
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        animator.SetBool("IsGround", isGrounded);
    }

    private void FlipSprite()
    {
        if (moveInput.x > 0 && !facingRight)
            Flip();
        else if (moveInput.x < 0 && facingRight)
            Flip();
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    public void OnColliderEnter(Collider2D col)
    {
        LOG.trace(col.name);
    }
    public void OnColliderLeave(Collider2D col)
    {
        LOG.trace(col.name);
    }
}
