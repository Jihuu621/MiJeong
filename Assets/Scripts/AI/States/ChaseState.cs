using UnityEngine;

public class ChaseState : IEnemyState
{
    Transform _player;
    float _chaseSpeed;
    float _chaseRange = 15f; // ���� ���� �ݰ�
    float _attackRange = 1.5f; // ���� ��ȯ �Ÿ�

    public void EnterState(EnemyStateManager enemy)
    {
        enemy.GetComponent<SpriteRenderer>().color = Color.red;
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;
        _chaseSpeed = enemy.GetComponent<EnemyDataManager>().EnemyData.MoveSpeed;
    }

    public void ExitState(EnemyStateManager enemy)
    {
        // Debug.Log("[Chase State] : Exit");
    }

    public void UpdateState(EnemyStateManager enemy)
    {
        if (_player == null)
        {
            enemy.TransitionToState(new IdleState());
            return;
        }

        float dist = Vector2.Distance(enemy.transform.position, _player.position);

        // �÷��̾ ���� �ݰ��� ����� Patrol�� ��ȯ
        if (dist > _chaseRange)
        {
            enemy.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            enemy.TransitionToState(new PatrolState());
            return;
        }

        // �÷��̾�� ����� ��������� AttackState�� ��ȯ
        if (dist < _attackRange)
        {
            enemy.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            enemy.TransitionToState(new AttackState());
            return;
        }

        // �÷��̾ ���� �̵� �� ���� ��ȯ
        float moveDir = _player.position.x > enemy.transform.position.x ? 1f : -1f;
        enemy.transform.localScale = new Vector3(moveDir, enemy.transform.localScale.y, enemy.transform.localScale.z);
        enemy.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(moveDir * _chaseSpeed, 0f);
    }
}