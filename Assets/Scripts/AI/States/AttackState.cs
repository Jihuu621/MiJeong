using UnityEngine;

public class AttackState : IEnemyState
{
    float _attackRange = 1.5f; // 공격 사거리
    float _attackDelay = 1.0f; // 공격 간격
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

        // 플레이어가 공격 범위 밖으로 벗어나면 ChaseState로 전환
        if (dist > _attackRange)
        {
            enemy.TransitionToState(new ChaseState());
            return;
        }

        // 공격 쿨타임 체크
        _timer += Time.deltaTime;
        if (_timer >= _attackDelay)
        {
            // 플레이어의 Health 컴포넌트에 데미지 적용
            Health playerHealth = _player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(_damage);
                Debug.Log($"적이 플레이어를 공격! (데미지: {_damage})");
            }
            _timer = 0f;
        }
    }
}