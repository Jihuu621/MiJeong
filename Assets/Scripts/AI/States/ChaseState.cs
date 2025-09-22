using UnityEngine;

public class ChaseState : IEnemyState
{
    public void EnterState(EnemyStateManager enemy)
    {
        Debug.Log("[Chase State] : Enter");
    }

    public void ExitState(EnemyStateManager enemy)
    {
        Debug.Log("[Chase State] : Exit");
    }

    public void UpdateState(EnemyStateManager enemy)
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            enemy.TransitionToState(new AttackState());
    }
}
