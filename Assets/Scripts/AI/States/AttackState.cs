using UnityEngine;

public class AttackState : IEnemyState
{
    public void EnterState(EnemyStateManager enemy)
    {
        //Debug.Log("[Attack State] : Enter");
    }

    public void ExitState(EnemyStateManager enemy)
    {
        //Debug.Log("[Attack State] : Exit");
    }

    public void UpdateState(EnemyStateManager enemy)
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            enemy.TransitionToState(new IdleState());
    }
}
