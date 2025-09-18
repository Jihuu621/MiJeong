// PlayerController.cs
// Unity 2D 플랫포머 이동 시스템
// 플레이어 오브젝트에 부착해야 함. 필요한 컴포넌트:
//  - Rigidbody2D (Body Type = Dynamic, Z축 회전 고정)
//  - Collider2D (CapsuleCollider2D 또는 BoxCollider2D)
//  - 발밑/벽 체크용 Transform 자식 오브젝트
// 기능:
//  - 부드러운 좌우 이동 (가속 / 감속)
//  - 가변 점프 높이 (점프 키를 오래 누르면 높이 점프)
//  - 코요티 타임 (땅에서 떨어진 직후 잠시 점프 가능)
//  - 점프 버퍼링 (착지 직전 미리 점프 입력 가능)
//  - 벽 미끄러짐 및 벽 점프
//  - 선택적 대시 기능 (쿨다운 포함)
// Unity의 구 Input System (Input.GetAxis / GetButton) 기반. 새 Input System 사용 시 입력 부분 수정 필요.

using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("이동")]
    public float moveSpeed = 8f;                // 최대 속도
    public float acceleration = 60f;            // 최고 속도에 도달하는 속도
    public float deceleration = 60f;            // 멈추는 속도
    public float velPower = 0.9f;               // 가속 반응 조절

    [Header("점프")]
    public float jumpForce = 18f;               // 초기 점프 힘
    public float jumpCutMultiplier = 0.5f;      // 점프 도중 키를 떼면 줄어드는 비율
    public int extraJumps = 0;                  // 추가 점프 횟수 (0 = 없음)

    [Header("점프 보정")]
    public float gravityScale = 3f;             // 기본 중력 배율
    public float fallGravityMultiplier = 2.5f;  // 낙하 중 추가 중력 배율

    [Header("땅 & 벽 체크")]
    public Transform groundCheck;               // 발밑 체크 위치
    public float groundCheckRadius = 0.08f;
    public LayerMask groundLayer;

    public Transform wallCheck;                 // 벽 체크 위치
    public float wallCheckDistance = 0.2f;

    [Header("코요티 & 버퍼")]
    public float coyoteTime = 0.12f;            // 땅에서 떨어진 뒤 점프 가능 시간
    public float jumpBufferTime = 0.12f;        // 점프 입력을 미리 받아두는 시간

    [Header("벽 미끄러짐/점프")]
    public float wallSlideSpeed = 2.5f;         // 벽 미끄러짐 속도
    public Vector2 wallJumpVelocity = new Vector2(12f, 18f); // 벽 점프 힘
    public float wallJumpDuration = 0.18f;      // 벽 점프 후 입력 잠금 시간

    [Header("대시 (옵션)")]
    public bool allowDash = false;
    public float dashSpeed = 20f;               // 대시 속도
    public float dashDuration = 0.14f;          // 대시 지속 시간
    public float dashCooldown = 0.6f;           // 대시 쿨다운

    // 내부 변수들
    Rigidbody2D rb;
    float horizontal;
    bool facingRight = true;

    // 땅/벽 상태
    bool isGrounded;
    bool isTouchingWall;
    int wallDir; // -1 = 왼쪽, 1 = 오른쪽

    // 점프 상태
    float coyoteTimer;
    float jumpBufferTimer;
    int jumpsRemaining;

    // 벽 점프 상태
    bool isWallSliding;
    float wallJumpTimer;

    // 대시 상태
    bool isDashing;
    float dashTimer;
    float dashCooldownTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
        jumpsRemaining = extraJumps;
    }

    void Update()
    {
        // --- 입력 (구 Input System) ---
        horizontal = Input.GetAxisRaw("Horizontal"); // -1,0,1

        // 점프 입력
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferTimer = jumpBufferTime;
        }
        else
        {
            jumpBufferTimer -= Time.deltaTime;
        }

        if (Input.GetButtonUp("Jump"))
        {
            // 가변 점프: 점프 중간에 떼면 속도 줄이기
            if (rb.linearVelocity.y > 0f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
            }
        }

        // 대시 입력
        if (allowDash && Input.GetButtonDown("Fire3") && dashCooldownTimer <= 0f && !isDashing)
        {
            StartDash();
        }

        // 타이머
        if (isGrounded) coyoteTimer = coyoteTime; else coyoteTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        CheckSurroundings();

        if (isDashing)
        {
            HandleDash();
            return;
        }

        if (wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.fixedDeltaTime;
        }

        HandleMovement();
        HandleJumping();
        HandleWallSlide();

        if (dashCooldownTimer > 0f) dashCooldownTimer -= Time.fixedDeltaTime;
    }

    void CheckSurroundings()
    {
        // 땅 체크
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // 벽 체크 (좌우 레이캐스트)
        RaycastHit2D leftHit = Physics2D.Raycast(transform.position, Vector2.left, wallCheckDistance, groundLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(transform.position, Vector2.right, wallCheckDistance, groundLayer);
        isTouchingWall = leftHit.collider != null || rightHit.collider != null;
        if (leftHit.collider != null) wallDir = -1; else if (rightHit.collider != null) wallDir = 1; else wallDir = 0;
    }

    void HandleMovement()
    {
        // 벽 점프 후 잠시 동안 좌우 입력 무시
        if (wallJumpTimer > 0f) horizontal = 0f;

        float targetSpeed = horizontal * moveSpeed;
        float speedDiff = targetSpeed - rb.linearVelocity.x;

        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, velPower) * Mathf.Sign(speedDiff);

        rb.AddForce(new Vector2(movement, 0f));

        // 속도 제한
        if (Mathf.Abs(rb.linearVelocity.x) > moveSpeed && Mathf.Sign(rb.linearVelocity.x) == Mathf.Sign(targetSpeed))
        {
            rb.linearVelocity = new Vector2(Mathf.Sign(rb.linearVelocity.x) * moveSpeed, rb.linearVelocity.y);
        }

        // 캐릭터 방향 전환
        if (horizontal > 0.1f && !facingRight) Flip();
        else if (horizontal < -0.1f && facingRight) Flip();
    }

    void HandleJumping()
    {
        // 점프 버퍼 & 코요티 타임
        if (jumpBufferTimer > 0f)
        {
            if (coyoteTimer > 0f || jumpsRemaining > 0 || isWallSliding)
            {
                if (isWallSliding)
                {
                    // 벽 점프: 벽 반대 방향으로 튀기기
                    rb.linearVelocity = new Vector2(-wallDir * wallJumpVelocity.x, wallJumpVelocity.y);
                    wallJumpTimer = wallJumpDuration;
                    isWallSliding = false;
                }
                else
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                    if (!isGrounded && jumpsRemaining > 0)
                    {
                        jumpsRemaining--;
                    }
                }

                jumpBufferTimer = 0f;
                coyoteTimer = 0f;
            }
        }

        // 착지 시 추가 점프 초기화
        if (isGrounded && !Input.GetButton("Jump"))
        {
            jumpsRemaining = extraJumps;
        }

        // 낙하 중 중력 증가
        if (rb.linearVelocity.y < 0f)
        {
            rb.gravityScale = gravityScale * fallGravityMultiplier;
        }
        else
        {
            rb.gravityScale = gravityScale;
        }
    }

    void HandleWallSlide()
    {
        isWallSliding = false;

        if (isTouchingWall && !isGrounded && rb.linearVelocity.y < 0f)
        {
            isWallSliding = true;
            // 낙하 속도 제한
            if (rb.linearVelocity.y < -wallSlideSpeed)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlideSpeed);
            }
        }
    }

    void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;

        // 대시 중에는 중력 제거
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2((facingRight ? 1f : -1f) * dashSpeed, 0f);
    }

    void HandleDash()
    {
        dashTimer -= Time.fixedDeltaTime;
        if (dashTimer <= 0f)
        {
            isDashing = false;
            rb.gravityScale = gravityScale;
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 s = transform.localScale;
        s.x *= -1f;
        transform.localScale = s;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.left * wallCheckDistance);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * wallCheckDistance);
    }
}

/* 사용법:
1. Player 오브젝트 생성 후 Rigidbody2D (Z축 회전 고정) 및 Collider2D 추가.
2. 발밑에 "GroundCheck" 빈 오브젝트 추가 (feet 위치), Inspector에서 groundCheck로 지정.
3. LayerMask로 groundLayer 설정.
4. Inspector에서 속도, 점프력 등 수치 조정.
5. Input 설정: Horizontal (A/D 또는 화살표), Jump (Space), Fire3 (Shift) — 대시용.

추가 팁:
- 새 Input System 사용 시 Input 코드 부분 교체 필요.
- 애니메이션 연동: Speed, IsGrounded, IsWallSliding, VerticalVelocity 등을 Animator 파라미터로 활용.
- 대시 기능을 쓰지 않을 경우 allowDash를 false로.
*/