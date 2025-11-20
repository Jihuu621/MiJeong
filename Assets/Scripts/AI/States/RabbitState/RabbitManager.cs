using UnityEngine;

public class RabbitManager : MonoBehaviour
{
    public IRabbitState CurrentState;

    private void Start()
    {
        TransitionToState(new Rabbit_Idle());
    }

    private void Update()
    {
        CurrentState?.UpdateState(this);
    }

    public void TransitionToState(IRabbitState newState)
    {
        CurrentState?.ExitState(this);
        CurrentState = newState;
        CurrentState.EnterState(this);
        //print($"[TransitionToState] {newState}로 스테이츠 변경");
    }

}
