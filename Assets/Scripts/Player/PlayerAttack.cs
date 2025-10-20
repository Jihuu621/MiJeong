using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    [Header("공격 설정")]
    public BoxCollider2D hitbox;   // 공격 판정용 (IsTrigger 체크)
    public float comboDelay = 0.6f; // 콤보 유지 가능 시간

    [Header("데미지 설정")]
    public int damageA = 10;
    public int damageB = 10;
    public int damageC = 15;

    private int comboStep = 0;          // 현재 콤보 단계
    private float comboTimer = 0f;      // 콤보 타이머
    private bool isAttacking = false;   // 공격 중 여부
    private bool canNextCombo = false;  // 다음 콤보 입력 가능 여부
    private bool comboQueued = false;   // 다음 콤보 입력이 대기 중인지
    private int currentDamage = 0;      // 현재 공격의 데미지

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        hitbox.enabled = false;

        // 자기 자신과 충돌 무시 (필요 시 태그로 조정)
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), hitbox, true);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isAttacking)
                Attack();
            else if (canNextCombo)
                comboQueued = true; // 다음 공격 입력 저장
        }

        // 공격 중이 아닐 때만 콤보 타이머 흐름
        if (!isAttacking && comboStep > 0)
        {
            comboTimer += Time.deltaTime;
            if (comboTimer > comboDelay)
                ResetCombo();
        }
    }

    void Attack()
    {
        if (isAttacking) return;
        isAttacking = true;
        comboQueued = false;

        // 현재 단계에 맞는 공격 선택
        switch (comboStep)
        {
            case 0:
                StartCoroutine(DoAttack("A", damageA));
                break;
            case 1:
                StartCoroutine(DoAttack("B", damageB));
                break;
            case 2:
                StartCoroutine(DoAttack("C", damageC));
                break;
        }
    }

    IEnumerator DoAttack(string attackType, int damage)
    {
        currentDamage = damage;
        Debug.Log($"공격 {attackType} 발동! (데미지 {damage})");

        // 애니메이션 트리거
        if (animator)
            animator.SetTrigger("Attack" + attackType);

        // 공격 판정 시작 전 약간의 준비 시간
        yield return new WaitForSeconds(0.1f);
        hitbox.enabled = true;

        // 히트박스 유지시간
        yield return new WaitForSeconds(0.15f);
        hitbox.enabled = false;

        // 콤보 입력 가능 구간
        canNextCombo = true;

        // 다음 공격 대기 시간
        float nextComboWindow = 0.3f;
        float elapsed = 0f;
        while (elapsed < nextComboWindow)
        {
            if (comboQueued)
            {
                comboStep++;
                if (comboStep > 2) comboStep = 0;
                canNextCombo = false;
                comboQueued = false;
                isAttacking = false;
                Attack(); // 다음 공격 실행
                yield break;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 다음 공격 입력이 없으면 종료
        canNextCombo = false;
        isAttacking = false;
        comboStep++;
        if (comboStep > 2) comboStep = 0;
        comboTimer = 0f;
    }

    void ResetCombo()
    {
        comboStep = 0;
        comboTimer = 0f;
        isAttacking = false;
        canNextCombo = false;
        comboQueued = false;
        Debug.Log("콤보 리셋");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hitbox.enabled) return;
        if (other.gameObject == gameObject) return;

        // 나중에 적 스크립트 완성되면 아래 사용
        // Enemy enemy = other.GetComponent<Enemy>();
        // if (enemy != null)
        // {
        //     enemy.TakeDamage(currentDamage);
        // }

        Debug.Log($"히트박스 충돌: {other.name} (데미지 {currentDamage})");
    }
}
