using UnityEngine;

public class PlayerGuard : MonoBehaviour
{
    [Header("가드 설정")]
    public float guardGauge = 100f;
    public float maxGuardGauge = 100f;
    public float guardRegenRate = 10f;
    public float guardHitCost = 20f;
    public float enemyKillRegen = 5f;
    public bool isGuarding;

    [Header("가드 브레이크")]
    public float knockbackForce = 10f;

    [Header("퍼펙트 가드 설정")]
    public float perfectGuardWindow = 0.2f; // 가드 시작 직후 이 시간 안에 공격 받으면 퍼펙트 가드
    public float perfectGuardCooldown = 0.4f; // 적 공격 이후 이 시간 동안은 퍼펙트 가드 불가

    private float lastGuardStartTime;
    private float lastEnemyAttackTime;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        isGuarding = Input.GetMouseButton(1);

        // 가드 시작 시점 기록
        if (Input.GetMouseButtonDown(1))
        {
            lastGuardStartTime = Time.time;
            Debug.Log($"가드 시작 ({lastGuardStartTime:F2})");
        }

        // 가드 중이 아닐 때만 게이지 회복
        if (!isGuarding)
        {
            guardGauge = Mathf.Min(maxGuardGauge, guardGauge + guardRegenRate * Time.deltaTime);
        }
    }

    public void NotifyEnemyAttack()
    {
        // 적이 공격을 시도했을 때 (TakeDamage 호출 직전) 이걸 호출
        lastEnemyAttackTime = Time.time;
        Debug.Log($"적 공격 감지 ({lastEnemyAttackTime:F2})");
    }

    public bool TryPerfectGuard()
    {
        // 공격 이후 일정 시간(쿨타임) 동안은 퍼펙트 가드 불가능
        if (Time.time - lastEnemyAttackTime < perfectGuardCooldown)
        {
            Debug.Log("퍼펙트 가드 쿨타임 중 (일반 가드로 처리)");
            return false;
        }

        // 가드 시작 후 일정 시간 안에 공격을 받으면 퍼펙트 가드 성공
        bool success = Time.time - lastGuardStartTime <= perfectGuardWindow;
        if (success)
            Debug.Log("퍼펙트 가드 성공!");
        else
            Debug.Log(" 일반 가드 처리");

        return success;
    }

    public float GetDamageAfterGuard(float damage)
    {
        // 일반 가드 피해 감소 60%
        return damage * 0.4f;
    }

    public void UseGuardGauge(float amount)
    {
        guardGauge = Mathf.Max(guardGauge - amount, 0f);
        Debug.Log($"가드 게이지 감소: -{amount}, 현재 게이지 = {guardGauge}");
    }

    public void ApplyGuardBreakKnockback(PlayerController player)
    {
        if (rb == null) return;

        float direction = player.transform.localScale.x > 0 ? -1f : 1f;
        rb.linearVelocity = new Vector2(direction * knockbackForce, rb.linearVelocity.y);
    }

    public void OnEnemyKilled()
    {
        guardGauge = Mathf.Min(maxGuardGauge, guardGauge + enemyKillRegen);
    }
}
