using UnityEngine;

public class AttackState : IEnemyState
{
    float _attackRange = 1.5f;   // 공격 사거리
    float _attackDelay = 1.0f;   // 공격 간격
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
            // 플레이어 Health 가져오기
            Health playerHealth = _player.GetComponent<Health>();
            if (playerHealth != null)
            {
                float finalDamage = _damage;

                // 플레이어 패링/가드 시스템 확인
                PlayerParry parry = _player.GetComponent<PlayerParry>();
                if (parry != null)
                {
                    // parry.OnHit 안에서
                    //  - 패링 시간: 데미지 0, 스턴, 게이지 +5
                    //  - 가드 시간: 데미지 60% 감소, 게이지 -20
                    //  - 그 외: 데미지 그대로
                    finalDamage = parry.OnHit(enemy, _damage);
                }

                if (finalDamage > 0f)
                {
                    playerHealth.TakeDamage(finalDamage);
                    Debug.Log($"<color=red>[적] 플레이어를 공격! ({finalDamage} 피해)</color>");
                }
                else
                {
                    Debug.Log("<color=green>[적] 공격이 패링/완전 방어되었습니다. (피해 0)</color>");
                }
            }

            _timer = 0f;
        }
    }
}
