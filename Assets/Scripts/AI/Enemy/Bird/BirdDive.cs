using UnityEngine;

public class BirdDive : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 3f;
    public float damage = 20f;
    public Vector2 hitKnockback = new Vector2(0f, 5f);
    public float knockbackDuration = 0.2f;
    public float steerSpeed = 3f; // 목표 방향으로 바뀌는 속도(클수록 빠르게 직선에 근접)
    public float yThreshold = 0.12f; // straightY에 근접한 것으로 판단하는 허용치
    [Header("직선 전환 기준 Y (월드 Y)")]
    public float straightY = 0f; // 이 Y값에 도달하면 수평(직선)으로 전환

    private Vector2 moveDir;
    private bool reachedSameY = false;

    private Rigidbody2D rb;
    private Transform player;

    // 스포너 참조 (스폰한 새가 파괴되면 스포너에 알려주기 위함)
    [System.NonSerialized] public BirdSpawner spawnerRef;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // spawner 인자 추가 (null 허용)
    public void Init(Transform playerTransform, bool isRight, BirdSpawner spawner = null)
    {
        player = playerTransform;
        spawnerRef = spawner;
        float horizontal = isRight ? 1f : -1f;
        moveDir = new Vector2(horizontal, -0.7f).normalized;
        transform.localScale = new Vector3(isRight ? 1 : -1, 1, 1);
        Destroy(gameObject, lifeTime);
    }

    void FixedUpdate()
    {
        if (!reachedSameY)
        {
            // 목표 방향 계산: 플레이어가 있으면 플레이어를 기준으로, 없으면 현재 방향 유지하면서 아래쪽으로 향함
            Vector2 toTarget;
            if (player != null)
                toTarget = (Vector2)player.position - (Vector2)transform.position;
            else
                toTarget = new Vector2(Mathf.Sign(moveDir.x), -1f);

            Vector2 desired = new Vector2(Mathf.Sign(toTarget.x), toTarget.y).normalized;
            moveDir = Vector2.Lerp(moveDir, desired, steerSpeed * Time.fixedDeltaTime).normalized;

            // 지정된 straightY에 근접하면 수평(직선)으로 전환
            if (Mathf.Abs(transform.position.y - straightY) <= yThreshold)
            {
                reachedSameY = true;
                float horiz = (toTarget.x != 0f) ? Mathf.Sign(toTarget.x) : Mathf.Sign(moveDir.x);
                moveDir = new Vector2(horiz, 0f).normalized;
            }
        }
        else
        {
            // 이미 수평 전환된 상태: x 방향은 플레이어 상대 위치가 있으면 그것을 따르고, 없으면 기존 방향 유지
            float horizontal;
            if (player != null)
                horizontal = Mathf.Sign(player.position.x - transform.position.x);
            else
                horizontal = Mathf.Sign(moveDir.x);

            moveDir = new Vector2(horizontal, 0f).normalized;
        }

        // Rigidbody2D의 linearVelocity 사용 (프로젝트 네이밍과 일치)
        if (rb != null)
            rb.linearVelocity = moveDir * speed;

        // 플립 (시각)
        if (moveDir.x != 0f)
        {
            Vector3 s = transform.localScale;
            s.x = Mathf.Sign(moveDir.x) * Mathf.Abs(s.x);
            transform.localScale = s;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Health hp = collision.GetComponent<Health>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
            }

            PlayerController playerCtrl = collision.GetComponent<PlayerController>();
            if (playerCtrl != null && (hitKnockback != Vector2.zero) && knockbackDuration > 0f)
            {
                playerCtrl.ApplyKnockback(hitKnockback, knockbackDuration);
            }

            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        // 파괴될 때 스포너에게 알려 activeBird를 해제하게 함
        if (spawnerRef != null)
            spawnerRef.UnregisterBird(gameObject);
    }
}
