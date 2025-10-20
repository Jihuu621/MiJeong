using UnityEngine;

public class EnemyAttackTest : MonoBehaviour
{
    [Header("공격 설정")]
    public float attackDamage = 25f;
    public float attackInterval = 2f;
    public float attackRange = 1.5f;
    public LayerMask playerLayer;

    [Header("디버그용")]
    public bool autoAttack = true;
    private float attackTimer;

    void Update()
    {
        if (autoAttack)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                TryAttack();
                attackTimer = attackInterval;
            }
        }

        // 수동 공격
        if (Input.GetKeyDown(KeyCode.T))
        {
            TryAttack();
        }
    }

    void TryAttack()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);
        if (hit != null)
        {
            Health health = hit.GetComponent<Health>();
            if (health != null)
            {
                // 퍼펙트 가드 타이밍 기준이 되도록 공격 직전에 알림
                PlayerGuard guard = hit.GetComponent<PlayerGuard>();
                if (guard != null)
                    guard.NotifyEnemyAttack();

                Debug.Log($"적이 플레이어 공격! (데미지 {attackDamage})");
                health.TakeDamage(attackDamage);
            }
        }
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
