using UnityEngine;
using System.Collections;

public class PlayerParry : MonoBehaviour
{
    [Header("패링/가드 구간 설정")]
    public float failWindow = 0.7f;
    public float parryWindow = 1.0f;
    public float guardWindow = 1.0f;

    public float parryCooldown = 0.5f;
    public float guardDamageReduce = 0.6f;

    [Header("쿨타임 관련설정")]
    private bool isFailTime = false;
    private bool isParryTime = false;
    private bool isGuardTime = false;
    private bool isCooldown = false;

    [Header("게이지 설정")]
    public float maxGauge = 100f;
    public float currentGauge;
    public float gaugeRegenPerSec = 1f;

    public float parryReward = 5f;
    public float guardPenalty = 20f;

    //디버깅용 Getter
    public bool IsFailTime => isFailTime;
    public bool IsParryTime => isParryTime;
    public bool IsGuardTime => isGuardTime;

    void Start()
    {
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
            Debug.Log("<color=grey>[플레이어] 쿨타임 상태!</color>");
            return;
        }

        if (currentGauge < 5f)
        {
            Debug.Log("<color=red>[플레이어] 게이지 부족!</color>");
            return;
        }

        StartCoroutine(ParryGuardRoutine());
        StartCoroutine(CooldownRoutine());
    }

    // 총 2.7초 패링/가드 시스템
    IEnumerator ParryGuardRoutine()
    {
        //패링 실패 구간 (0.7초)
        isFailTime = true;
        isParryTime = false;
        isGuardTime = false;
        Debug.Log("<color=red>[플레이어] 패링 실패 구간 (0.7초)</color>");

        yield return new WaitForSeconds(failWindow);

        //패링 구간 (1초)
        isFailTime = false;
        isParryTime = true;
        Debug.Log("<color=cyan>[플레이어] 패링 가능 구간 (1초)</color>");

        yield return new WaitForSeconds(parryWindow);

        //가드 구간 (1초)
        isParryTime = false;
        isGuardTime = true;
        Debug.Log("<color=blue>[플레이어] 가드 구간 (1초)</color>");

        yield return new WaitForSeconds(guardWindow);

        // 종료
        isGuardTime = false;
        Debug.Log("<color=grey>[플레이어] 패링/가드 종료</color>");
    }

    // 쿨타임
    IEnumerator CooldownRoutine()
    {
        isCooldown = true;
        yield return new WaitForSeconds(parryCooldown);
        isCooldown = false;
        Debug.Log("<color=white>[플레이어] 패링 사용 가능!</color>");
    }

    // 데미지 처리
    public float OnHit(EnemyStateManager enemy, float damage)
    {
        if (isFailTime)
        {
            Debug.Log("<color=grey>[플레이어] 패링 실패 구간 - 데미지 그대로</color>");
            return damage;
        }

        if (isParryTime)
        {
            Debug.Log("<color=green>[플레이어] 패링 성공! 데미지 0 + 게이지 +5</color>");
            currentGauge = Mathf.Min(currentGauge + parryReward, maxGauge);
            enemy.TransitionToState(new StunState());
            return 0f;
        }

        if (isGuardTime)
        {
            float reducedDamage = damage * (1f - guardDamageReduce);
            currentGauge -= guardPenalty;

            Debug.Log($"<color=yellow>[플레이어] 가드 성공! 데미지 감소({reducedDamage}), 게이지 -20</color>");

            return reducedDamage;
        }

        return damage;
    }
}
