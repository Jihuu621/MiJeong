using UnityEngine;

public class Rabbit_Chase : IRabbitState
{
    Transform _player;
    float _chaseSpeed;
    float _chaseRange = 15f; // 추적 유지 반경
    float _attackRange = 1.5f; // 공격 전환 거리

    public void EnterState(RabbitManager enemy)
    {
        enemy.GetComponent<SpriteRenderer>().color = Color.red;
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;
        _chaseSpeed = enemy.GetComponent<EnemyDataManager>().EnemyData.MoveSpeed;
    }

    public void ExitState(RabbitManager enemy)
    {
        // Debug.Log("[Chase State] : Exit");
    }

    public void UpdateState(RabbitManager enemy)
    {
        if (_player == null)
        {
            enemy.TransitionToState(new Rabbit_Idle());
            return;
        }

        float dist = Vector2.Distance(enemy.transform.position, _player.position);

        // 플레이어가 추적 반경을 벗어나면 Patrol로 전환
        if (dist > _chaseRange)
        {
            enemy.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            enemy.TransitionToState(new Rabbit_Patrol());
            return;
        }

        // 플레이어와 충분히 가까워지면 AttackState로 전환
        if (dist < _attackRange)
        {
            enemy.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            enemy.TransitionToState(new Rabbit_Attack());
            return;
        }

        // 플레이어를 향해 이동 및 방향 전환
        float moveDir = _player.position.x > enemy.transform.position.x ? 1f : -1f;

        // 변경: x 스케일의 절대값은 유지하고 부호만 바꿉니다.
        Vector3 ls = enemy.transform.localScale;
        float absX = Mathf.Abs(ls.x);
        ls.x = absX * (moveDir >= 0f ? 1f : -1f);
        enemy.transform.localScale = ls;

        enemy.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(moveDir * _chaseSpeed, 0f);
    }
}