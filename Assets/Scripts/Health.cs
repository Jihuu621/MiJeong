using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHP = 100f;
    public float currentHP;
    public System.Action OnDeath; // 죽었을 때 이벤트
    public EnemyData enemyData;

    private void Awake()
    {
        if (enemyData != null)
        {
            maxHP = enemyData.MaxHP;
            currentHP = maxHP;
        }
    }

    public void SetMaxHP(float max)
    {
        maxHP = max;
        currentHP = maxHP;
    }

    public void TakeDamage(float damage)
    {
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
            Debug.Log("유다히");
            
        }
        else if (CompareTag("Enemy"))
        {
            Debug.Log("적 사망");
            Destroy(gameObject);
        }
    }
}
