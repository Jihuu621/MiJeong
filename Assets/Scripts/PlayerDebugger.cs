using UnityEngine;

public class PlayerDebugger : MonoBehaviour
{
    public PlayerParry parry;
    public float debugRadius = 1.5f;

    private void OnDrawGizmos()
    {
        if (parry == null) return;

        Gizmos.color = parry.IsParrying ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, debugRadius);
    }
}
