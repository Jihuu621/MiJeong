using UnityEngine;

public class EnemyStateManager : MonoBehaviour
{
    public IEnemyState CurrentState;

    private void Start()
    {
        TransitionToState(new IdleState());
    }

    private void Update()
    {
        CurrentState?.UpdateState(this);
    }

    public void TransitionToState(IEnemyState newState)
    {
        CurrentState?.ExitState(this);
        CurrentState = newState;
        CurrentState.EnterState(this);
        //print($"[TransitionToState] {newState}�� �������� ����");
    }

}
