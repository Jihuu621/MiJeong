using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHP = 100f;
    public float currentHP;
    public System.Action OnDeath;
    public EnemyData enemyData;

    private void Awake()
    {
        if (CompareTag("Enemy") && enemyData != null)
        {
            maxHP = enemyData.MaxHP;
        }

        // Player�� Enemy �� �� ü�� �⺻ ����
        currentHP = maxHP;
    }


    public void SetMaxHP(float max)
    {
        maxHP = max;
        currentHP = maxHP;
    }

    public void TakeDamage(float damage)
    {
        PlayerGuard guard = GetComponent<PlayerGuard>();

        if (CompareTag("Player") && guard != null)
        {
            guard.NotifyEnemyAttack();

            if (guard.isGuarding)
            {
                bool perfect = guard.TryPerfectGuard();

                if (perfect)
                {
                    Debug.Log("�Ϻ� ���� ���� - ���� ����");
                    return;
                }

                // �Ϲ� ����
                if (guard.guardGauge > 0)
                {
                    damage = guard.GetDamageAfterGuard(damage);
                    guard.UseGuardGauge(guard.guardHitCost);
                }
                else
                {
                    // ������������˹�
                    PlayerController player = GetComponent<PlayerController>();
                    if (player != null)
                        guard.ApplyGuardBreakKnockback(player);
                }
            }
        }

        currentHP -= damage;
        Debug.Log($"{gameObject.name} HP: {currentHP}");

        if (currentHP <= 0)
        {
            currentHP = 0;
            Die();
        }
    }




    public void Heal(float amount)
    {
        currentHP = Mathf.Min(currentHP + amount, maxHP);
        Debug.Log($"{gameObject.name} HP: {currentHP}");
    }

    private void Die()
    {
        OnDeath?.Invoke();

        if (CompareTag("Player"))
        {
            Debug.Log("������");
            
        }
        else if (CompareTag("Enemy"))
        {
            Debug.Log("�� ���");
            Destroy(gameObject);
        }
    }
}
