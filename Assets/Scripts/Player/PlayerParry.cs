using UnityEngine;
using System.Collections;

public class PlayerParry : MonoBehaviour
{
    [Header("패링/가드 구간 설정")]
    public float failWindow = 0.7f;
    public float parryWindow = 1.0f;
    public float guardWindow = 1.0f;

    [Header("패링/가드 옵션")]
    public float parryCooldown = 0.5f;
    public float guardDamageReduce = 0.6f;

    [Header("게이지 설정")]
    public float maxGauge = 100f;
    public float currentGauge;
    public float gaugeRegenPerSec = 1f;
    public float parryReward = 5f;
    public float guardPenalty = 20f;

    private bool isFailTime = false;
    private bool isParryTime = false;
    private bool isGuardTime = false;
    private bool isCooldown = false;
    private bool isStunned = false;

    public bool IsFailTime => isFailTime;
    public bool IsParryTime => isParryTime;
    public bool IsGuardTime => isGuardTime;
    public bool IsStunned => isStunned;

    void Start()
    {
        currentGauge = maxGauge;
    }

    void Update()
    {
        if (isStunned) return;

        currentGauge += gaugeRegenPerSec * Time.deltaTime;
        currentGauge = Mathf.Clamp(currentGauge, 0f, maxGauge);

        if (Input.GetKeyDown(KeyCode.LeftControl))
            TryActivateParryGuard();
    }

    void TryActivateParryGuard()
    {
        if (isCooldown)
        {
            Debug.Log("<color=grey>[플레이어] 패링 쿨타임!</color>");
            return;
        }
        if (isStunned) return;
        if (currentGauge < 5f)
        {
            Debug.Log("<color=red>[플레이어] 패링 게이지 부족!</color>");
            return;
        } 
        currentGauge -= 5f;
        Debug.Log("<color=yellow>[플레이어] 패링 발동! 게이지 -5</color>");

        StartCoroutine(ParryGuardRoutine());
        StartCoroutine(CooldownRoutine());
    }


    IEnumerator ParryGuardRoutine()
    {
        isFailTime = true;
        isParryTime = false;
        isGuardTime = false;
        Debug.Log("<color=red>[플레이어] 패링 실패 구간 (0.7초)</color>");
        yield return new WaitForSeconds(failWindow);

        isFailTime = false;
        isParryTime = true;
        Debug.Log("<color=cyan>[플레이어] 패링 가능 구간 (1초)</color>");
        yield return new WaitForSeconds(parryWindow);

        isParryTime = false;
        isGuardTime = true;
        Debug.Log("<color=blue>[플레이어] 가드 구간 (1초)</color>");
        yield return new WaitForSeconds(guardWindow);

        isGuardTime = false;
        Debug.Log("<color=grey>[플레이어] 패링/가드 종료</color>");
    }

    IEnumerator CooldownRoutine()
    {
        isCooldown = true;
        yield return new WaitForSeconds(parryCooldown);
        isCooldown = false;
        Debug.Log("<color=white>[플레이어] 패링 재사용 가능</color>");
    }

    IEnumerator StunRoutine()
    {
        isStunned = true;
        Debug.Log("<color=purple>[플레이어] 기절 상태 (1초)</color>");
        yield return new WaitForSeconds(1f);
        isStunned = false;
        Debug.Log("<color=white>[플레이어] 기절 회복</color>");
    }

    public float OnHit(EnemyStateManager enemy, float damage)
    {
        if (isFailTime)
        {
            Debug.Log("<color=grey>[플레이어] 패링 실패 구간 - 데미지 그대로</color>");
            return damage;
        }

        if (isParryTime)
        {
            Debug.Log("<color=green>[플레이어] 패링 성공! 데미지 0 / 게이지 +5</color>");
            currentGauge = Mathf.Min(currentGauge + parryReward, maxGauge);
            enemy.TransitionToState(new StunState());
            return 0f;
        }

        if (isGuardTime)
        {
            float reducedDamage = damage * (1f - guardDamageReduce);
            currentGauge -= guardPenalty;
            Debug.Log($"<color=yellow>[플레이어] 가드! 데미지 {reducedDamage}, 게이지 -20</color>");

            if (currentGauge <= 0f)
            {
                currentGauge = 0f;
                StartCoroutine(StunRoutine());
            }

            return reducedDamage;
        }

        return damage;
    }
}
