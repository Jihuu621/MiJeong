using UnityEngine;

public class Rabbit_Idle : IRabbitState
{

    float _idletime;
    float _timer;

    public void EnterState(RabbitManager enemy)
    {
        enemy.GetComponent<SpriteRenderer>().color = Color.white;
        _idletime = Random.Range(1f, 4f);
        _timer = 0f;
    }

    public void ExitState(RabbitManager enemy)
    {
        //Debug.Log("[Idle State] : Exit");
    }

    public void UpdateState(RabbitManager enemy)
    {
        _timer += Time.deltaTime;
        if (_timer >= _idletime) {
            enemy.TransitionToState(new Rabbit_Patrol());
            return;
        }
    }
}
