using UnityEngine;

public class AttackState : IEnemyState
{
    float _attackRange = 1.5f; // ���� ��Ÿ�
    float _attackDelay = 1.0f; // ���� ����
    float _timer = 0f;
    Transform _player;
    float _damage;

    public void EnterState(EnemyStateManager enemy)
    {
        enemy.GetComponent<SpriteRenderer>().color = Color.magenta;
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;
        _damage = enemy.GetComponent<EnemyDataManager>().EnemyData.Damage;
        _timer = 0f;
    }

    public void ExitState(EnemyStateManager enemy)
    {
        // Debug.Log("[Attack State] : Exit");
    }

    public void UpdateState(EnemyStateManager enemy)
    {
        if (_player == null)
        {
            enemy.TransitionToState(new IdleState());
            return;
        }

        float dist = Vector2.Distance(enemy.transform.position, _player.position);

        // �÷��̾ ���� ���� ������ ����� ChaseState�� ��ȯ
        if (dist > _attackRange)
        {
            enemy.TransitionToState(new ChaseState());
            return;
        }

        // ���� ��Ÿ�� üũ
        _timer += Time.deltaTime;
        if (_timer >= _attackDelay)
        {
            // �÷��̾��� Health ������Ʈ�� ������ ����
            Health playerHealth = _player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(_damage);
                Debug.Log($"���� �÷��̾ ����! (������: {_damage})");
            }
            _timer = 0f;
        }
    }
}