using UnityEngine;
using System.Collections;

public class PlayerParry : MonoBehaviour
{
    [Header("패링/가드 설정")]
    public float parryWindow = 1.5f;      // 패링 구간 1.5초
    public float guardWindow = 1.5f;      // 가드 구간 1.5초

    public float parryCooldown = 0.5f;    // 쿨타임
    public float guardDamageReduce = 0.6f;

    private bool isParryTime = false;
    private bool isGuardTime = false;
    private bool isCooldown = false;

    [Header("게이지 설정")]
    public float maxGauge = 100f;
    public float currentGauge;
    public float gaugeRegenPerSec = 1f;

    public float parryReward = 5f;   // 패링 성공 시 +5
    public float guardPenalty = 20f; // 가드 시 -20

    public bool IsParrying => isParryTime;

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        currentGauge = maxGauge;
    }

    void Update()
    {
        currentGauge += gaugeRegenPerSec * Time.deltaTime;
        currentGauge = Mathf.Clamp(currentGauge, 0f, maxGauge);

        if (Input.GetKeyDown(KeyCode.LeftControl))
            TryActivateParryGuard();
    }

    // 패링/가드 시도
    void TryActivateParryGuard()
    {
        if (isCooldown)
        {
            Debug.Log("<color=grey>[플레이어] 쿨타임입니다!</color>");
            return;
        }

        if (currentGauge < 5f)
        {
            Debug.Log("<color=red>[플레이어] 게이지 부족으로 패링/가드 사용 불가!</color>");
            return;
        }

        StartCoroutine(ParryGuardRoutine());
        StartCoroutine(CooldownRoutine());
    }

    // 패링 → 가드 순서로 실행 (총 3초)
    IEnumerator ParryGuardRoutine()
    {
        // 1) 패링 구간 1.5초
        isParryTime = true;
        isGuardTime = false;
        sr.color = Color.cyan;
        Debug.Log("<color=cyan>[플레이어] 패링 구간 시작 (1.5초)</color>");

        yield return new WaitForSeconds(parryWindow);

        // 2) 가드 구간 1.5초
        isParryTime = false;
        isGuardTime = true;
        sr.color = Color.blue;
        Debug.Log("<color=blue>[플레이어] 가드 구간 시작 (1.5초)</color>");

        yield return new WaitForSeconds(guardWindow);

        // 종료
        isGuardTime = false;
        sr.color = Color.white;
        Debug.Log("<color=grey>[플레이어] 패링/가드 종료</color>");
    }

    // 쿨타임
    IEnumerator CooldownRoutine()
    {
        isCooldown = true;
        yield return new WaitForSeconds(parryCooldown);
        isCooldown = false;
        Debug.Log("<color=white>[플레이어] 패링/가드 다시 사용 가능!</color>");
    }

    // 적 공격이 들어왔을 때 처리
    public float OnHit(EnemyStateManager enemy, float damage)
    {
        // 1) 패링 성공 구간 (1.5초)
        if (isParryTime)
        {
            Debug.Log("<color=green>[플레이어] 패링 성공! 데미지 무효 + 게이지 +5</color>");
            currentGauge = Mathf.Min(currentGauge + parryReward, maxGauge);

            enemy.TransitionToState(new StunState());
            return 0f;
        }

        // 2) 가드 구간 (1.5초)
        if (isGuardTime)
        {
            float reducedDamage = damage * (1f - guardDamageReduce);
            currentGauge -= guardPenalty;

            Debug.Log($"<color=yellow>[플레이어] 가드 성공! 데미지 감소: {reducedDamage}, 게이지 -{guardPenalty}</color>");

            return reducedDamage;
        }

        // 3) 아무 상태 아님
        return damage;
    }
}
