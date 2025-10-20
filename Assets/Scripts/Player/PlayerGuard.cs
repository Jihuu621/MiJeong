using UnityEngine;

public class PlayerGuard : MonoBehaviour
{
    [Header("���� ����")]
    public float guardGauge = 100f;
    public float maxGuardGauge = 100f;
    public float guardRegenRate = 10f;
    public float guardHitCost = 20f;
    public float enemyKillRegen = 5f;
    public bool isGuarding;

    [Header("���� �극��ũ")]
    public float knockbackForce = 10f;

    [Header("����Ʈ ���� ����")]
    public float perfectGuardWindow = 0.2f; // ���� ���� ���� �� �ð� �ȿ� ���� ������ ����Ʈ ����
    public float perfectGuardCooldown = 0.4f; // �� ���� ���� �� �ð� ������ ����Ʈ ���� �Ұ�

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

        // ���� ���� ���� ���
        if (Input.GetMouseButtonDown(1))
        {
            lastGuardStartTime = Time.time;
            Debug.Log($"���� ���� ({lastGuardStartTime:F2})");
        }

        // ���� ���� �ƴ� ���� ������ ȸ��
        if (!isGuarding)
        {
            guardGauge = Mathf.Min(maxGuardGauge, guardGauge + guardRegenRate * Time.deltaTime);
        }
    }

    public void NotifyEnemyAttack()
    {
        // ���� ������ �õ����� �� (TakeDamage ȣ�� ����) �̰� ȣ��
        lastEnemyAttackTime = Time.time;
        Debug.Log($"�� ���� ���� ({lastEnemyAttackTime:F2})");
    }

    public bool TryPerfectGuard()
    {
        // ���� ���� ���� �ð�(��Ÿ��) ������ ����Ʈ ���� �Ұ���
        if (Time.time - lastEnemyAttackTime < perfectGuardCooldown)
        {
            Debug.Log("����Ʈ ���� ��Ÿ�� �� (�Ϲ� ����� ó��)");
            return false;
        }

        // ���� ���� �� ���� �ð� �ȿ� ������ ������ ����Ʈ ���� ����
        bool success = Time.time - lastGuardStartTime <= perfectGuardWindow;
        if (success)
            Debug.Log("����Ʈ ���� ����!");
        else
            Debug.Log(" �Ϲ� ���� ó��");

        return success;
    }

    public float GetDamageAfterGuard(float damage)
    {
        // �Ϲ� ���� ���� ���� 60%
        return damage * 0.4f;
    }

    public void UseGuardGauge(float amount)
    {
        guardGauge = Mathf.Max(guardGauge - amount, 0f);
        Debug.Log($"���� ������ ����: -{amount}, ���� ������ = {guardGauge}");
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
