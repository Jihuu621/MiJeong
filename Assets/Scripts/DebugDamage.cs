using UnityEngine;

public class DebugHealthControl : MonoBehaviour
{
    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
        if (health == null)
        {
            Debug.LogError("Health ������Ʈ�� ã�� �� �����ϴ�!");
        }
    }

    private void Update()
    {
        if (health == null) return;

        // KŰ ������ 10 ������
        if (Input.GetKeyDown(KeyCode.K))
        {
            health.TakeDamage(10f);
        }

        // LŰ ������ 10 ȸ��
        if (Input.GetKeyDown(KeyCode.L))
        {
            health.Heal(10f);
        }
    }
}
