using UnityEngine;
public class BirdSpawner : MonoBehaviour
{
    public GameObject birdPrefab;
    public Transform player;

    [Header("스폰 감지")]
    public Vector2 detectSize = new Vector2(8f, 4f);   // 직사각형 크기 (width, height)
    public Vector2 detectOffset = Vector2.zero;        // 박스 중심 오프셋 (로컬)
    public float detectAngle = 0f;                     // 박스 회전 각도 (도, 로컬 회전 추가)
    public LayerMask detectLayer = ~0;                 // 감지할 레이어 (기본: 전체) - 현재는 사용안함

    [Header("스폰 설정")]
    public float spawnInterval = 1f;     // 재스폰 최소 간격 (초)
    public float birdStraightY = 0f;     // 생성 시 Bird에게 설정할 straightY (월드 Y)
    private float timer;
    private GameObject activeBird;       // 현재 활성화된 새(최대 1마리)

    void Update()
    {
        timer += Time.deltaTime;

        if (birdPrefab == null) return;

        // player가 할당되지 않았으면 태그로 자동 찾기
        if (player == null)
        {
            GameObject pgo = GameObject.FindWithTag("Player");
            if (pgo != null) player = pgo.transform;
        }

        if (player == null) return;

        // 박스 내부 판정으로 변경
        bool playerInRange = IsPointInBox(player.position);

        // 플레이어가 박스 내에 있고 활성 새가 없으며 쿨다운이 끝났다면 스폰
        if (playerInRange && activeBird == null && timer >= spawnInterval)
        {
            SpawnBird();
            timer = 0f;
        }
    }

    void SpawnBird()
    {
        if (birdPrefab == null || player == null) return;
        bool isRight = player.position.x > transform.position.x;
        GameObject bird = Instantiate(birdPrefab, transform.position, Quaternion.identity);
        BirdDive dive = bird.GetComponent<BirdDive>();
        if (dive != null)
        {
            dive.Init(player, isRight, this);
            // Spawner에서 지정한 straightY가 있으면 덮어쓰기
            dive.straightY = birdStraightY;
        }
        activeBird = bird;
    }

    // Bird가 파괴될 때 Bird에서 호출하여 activeBird 정리
    public void UnregisterBird(GameObject bird)
    {
        if (activeBird == bird)
            activeBird = null;
    }

    // 월드 좌표의 점이 (로컬 오프셋 및 회전 적용된) 박스 내부에 있는지 검사
    private bool IsPointInBox(Vector2 worldPoint)
    {
        // 박스 중심: transform의 로컬 오프셋을 월드 좌표로 변환
        Vector2 boxCenter = transform.TransformPoint(detectOffset);

        // 박스 전체 회전: 객체의 Z 회전 + detectAngle
        float totalAngle = transform.eulerAngles.z + detectAngle;
        Quaternion rot = Quaternion.Euler(0f, 0f, totalAngle);

        // 점을 박스의 로컬 좌표계로 변환 (회전 역변환)
        Vector2 dir = worldPoint - boxCenter;
        Vector2 local = Quaternion.Inverse(rot) * dir;

        return Mathf.Abs(local.x) <= detectSize.x * 0.5f && Mathf.Abs(local.y) <= detectSize.y * 0.5f;
    }

    void OnDrawGizmosSelected()
    {
        Vector3 boxCenter = transform.TransformPoint(detectOffset);
        Matrix4x4 oldMat = Gizmos.matrix;
        Quaternion rot = Quaternion.Euler(0f, 0f, transform.eulerAngles.z + detectAngle);
        Gizmos.matrix = Matrix4x4.TRS(boxCenter, rot, Vector3.one);

        Color c = Color.red;
        if (player != null && IsPointInBox(player.position)) c = Color.green;
        Gizmos.color = c;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(detectSize.x, detectSize.y, 0.1f));
        Gizmos.matrix = oldMat;
        if (player != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}
