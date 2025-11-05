using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class ChessPiece : MonoBehaviour
{
    public float rotateWhileFalling = 180f;
    public float deactivateDelay = 2f; // 착지 후 비활성화까지 대기 시간
    private bool hasLanded = false;

    private Rigidbody2D rb;
    public Action onDeactivate; // PlayerSpawnChess에서 등록하는 콜백

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        hasLanded = false;
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 10f;
        }
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = true;
    }

    void Update()
    {
        if (!hasLanded && rotateWhileFalling != 0f)
        {
            transform.Rotate(Vector3.forward, rotateWhileFalling * Time.deltaTime);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasLanded) return;
        hasLanded = true;

        // 착지 후 물리 비활성화
        rb.bodyType = RigidbodyType2D.Kinematic;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // 일정 시간 후 비활성화 (Destroy 대신 SetActive(false))
        Invoke(nameof(Deactivate), deactivateDelay);
    }

    void Deactivate()
    {
        gameObject.SetActive(false);
        onDeactivate?.Invoke(); // PlayerSpawnChess에 알려줌
    }
}
