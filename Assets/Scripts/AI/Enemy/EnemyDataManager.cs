using UnityEngine;

public class EnemyDataManager : MonoBehaviour
{
    [SerializeField] private EnemyData _enemyData;  // �ν����Ϳ��� �Ҵ�

    // �ܺο��� ���� �� �ִ� ������Ƽ
    public EnemyData EnemyData => _enemyData;
}
