using UnityEngine;

public class EnemyDataManager : MonoBehaviour
{

    // [SerializeField]ÀÓ ¿ø·¡
    public EnemyData _enemyData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetEnemyData();
    }

    // Update is called once per frame
    void GetEnemyData()
    {
        print(
              $"Name : {_enemyData.EnemyName}\n" +
              $"HP : {_enemyData.MaxHP}\n" +
              $"Damage : {_enemyData.Damage}\n" +
              $"MoveSpeed : {_enemyData.MoveSpeed}\n" +
              $"Description : {_enemyData.EnemyDescription}\n"
        );
        
    }
}
