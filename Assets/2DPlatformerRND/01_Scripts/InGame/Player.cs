using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] GameObject MeleePrefab;
    [SerializeField] GameObject SkillPrefab;

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
    private bool isLock;
    private bool facingRight = true;

    public float currentHP = 100;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputActions = new PlayerInputActions();

        inputActions.Player.Jump.performed += ctx => OnJump();
        inputActions.Player.Attack.performed += ctx => OnAttack();
        inputActions.Player.Skill.performed += ctx => OnSkill();
        inputActions.Player.Dash.performed += ctx => OnDash();
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();

    private void Update()
    {
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        CheckGround();
        FlipSprite();
    }

    private void FixedUpdate()
    {
        if (isLock) return; // 대시 중에는 이동 입력 무시

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

    private void OnAttack()
    {
        if (isLock) return;

        isLock = true;
        this.ExDelayedCoroutine(0.8f, () => InstantiateMelee());
        this.ExDelayedCoroutine(1.0f, () => isLock = false);

        animator.CrossFade("PlayerMelee", 0);
        rb.linearVelocity = new Vector2(0f, 0f);
    }
    void InstantiateMelee()
    {
        // 스킬 오브젝트 생성
        Vector3 startPos = transform.position + new Vector3(facingRight ? 1f : -1f, 0, 0);
        GameObject melee = Instantiate(MeleePrefab, startPos, Quaternion.identity);
        Destroy(melee, 0.1f);
        melee.GetComponentInChildren<InteractableCollider>().OnInteractEnter.AddListener((col) =>
        {
            EnemyBase enemy = col.GetComponentInParent<EnemyBase>();
            if (enemy != null)
            {
                enemy.GetDamaged(10);
            }
        });
    }
    private void OnSkill()
    {
        if (isLock) return;

        isLock = true;

        this.ExDelayedCoroutine(0.5f, () =>
        {
            InstantiateSkill();
        });

        this.ExDelayedCoroutine(1, () => isLock = false);


        animator.CrossFade("PlayerSkill", 0);
        rb.linearVelocity = new Vector2(0f, 0f);
    }
    
    void InstantiateSkill()
    {
        // 스킬 오브젝트 생성
        GameObject skill = Instantiate(SkillPrefab, transform.position, Quaternion.identity);
        Vector3 destPos = transform.position + new Vector3(transform.localScale.x * 10, 0, 0);
        skill.transform.DOMove(destPos, 0.5f).OnComplete(() => Destroy(skill));
        skill.GetComponentInChildren<InteractableCollider>().OnInteractEnter.AddListener((col) =>
        {
            EnemyBase enemy = col.GetComponentInParent<EnemyBase>();
            if (enemy != null)
            {
                enemy.GetDamaged(20);
                skill.transform.DOKill();
                Destroy(skill);
            }
        });
    }

    private void OnDash()
    {
        if (isLock) return;

        isLock = true;
        this.ExDelayedCoroutine(dashDuration, () => isLock = false);

        animator.CrossFade("PlayerSkill", 0);

        float dashDir = facingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dashDir * dashForce, 0f);
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

    public void OnDamaged(Collider2D col)
    {
        currentHP -= 10f;
        if (currentHP <= 0)
        {
            animator.CrossFade("PlayerDeath", 0);
        }
        else
        {
            animator.CrossFade("PlayerHurt", 0);
        }
    }
}
