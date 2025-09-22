using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHP = 100f;
    public float currentHP;
    public System.Action OnDeath; // �׾��� �� �̺�Ʈ
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
            Debug.Log("������");
            
        }
        else if (CompareTag("Enemy"))
        {
            Debug.Log("�� ���");
            Destroy(gameObject);
        }
    }
}
