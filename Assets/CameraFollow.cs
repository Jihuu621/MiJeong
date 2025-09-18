using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;       // ���� �÷��̾�
    public float smoothSpeed = 0.125f; // ī�޶� �̵� �ε巯�� ����
    public Vector3 offset;         // ī�޶� ������ (Z�� ���� -10���� ����)

    private void FixedUpdate()
    {
        if (target == null) return;

        // ��ǥ ��ġ (�÷��̾��� X, Y�� ����, Z�� �״�� ��)
        Vector3 desiredPosition = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            transform.position.z // Z�� ī�޶� ������ ����
        );

        // �ε巴�� �̵�
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
