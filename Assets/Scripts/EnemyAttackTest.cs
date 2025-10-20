using UnityEngine;

public class EnemyAttackTest : MonoBehaviour
{
    [Header("���� ����")]
    public float attackDamage = 25f;
    public float attackInterval = 2f;
    public float attackRange = 1.5f;
    public LayerMask playerLayer;

    [Header("����׿�")]
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

        // ���� ����
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
                // ����Ʈ ���� Ÿ�̹� ������ �ǵ��� ���� ������ �˸�
                PlayerGuard guard = hit.GetComponent<PlayerGuard>();
                if (guard != null)
                    guard.NotifyEnemyAttack();

                Debug.Log($"���� �÷��̾� ����! (������ {attackDamage})");
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
