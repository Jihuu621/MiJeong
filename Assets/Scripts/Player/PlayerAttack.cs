﻿using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    [Header("공격 설정")]
    public BoxCollider2D hitbox;   // Square의 BoxCollider2D
    public SpriteRenderer hitboxRenderer; // Square의 SpriteRenderer
    public float comboDelay = 0.6f;

    [Header("디버그")]
    public bool showHitboxDebug = false; // 디버그용 히트박스 표시

    [Header("데미지 설정")]
    public int damageA = 10;
    public int damageB = 10;
    public int damageC = 15;

    private int comboStep = 0;
    private float comboTimer = 0f;
    private bool isAttacking = false;
    private bool canNextCombo = false;
    private bool comboQueued = false;
    private int currentDamage = 0;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        hitbox.enabled = false;
        if (hitboxRenderer != null)
            hitboxRenderer.enabled = false;

        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), hitbox, true);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isAttacking)
                Attack();
            else if (canNextCombo)
                comboQueued = true;
        }

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

        if (animator)
            animator.SetTrigger("Attack" + attackType);

        yield return new WaitForSeconds(0.1f);

        hitbox.enabled = true;
        if (hitboxRenderer != null)
            hitboxRenderer.enabled = showHitboxDebug;

        yield return new WaitForSeconds(0.15f);

        hitbox.enabled = false;
        if (hitboxRenderer != null)
            hitboxRenderer.enabled = false;

        canNextCombo = true;

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
                Attack();
                yield break;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

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

        Debug.Log($"히트박스 충돌: {other.name} (데미지 {currentDamage})");
    }
}