using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;          // 따라갈 캐릭터 (CshController 오브젝트)
    public Vector3 offset = new Vector3(0, 3, -5);
    public float smoothSpeed = 5f;
    public float mouseSensitivity = 2f;

    private float yaw = 0f;
    private float pitch = 10f;

    void LateUpdate()
    {
        if (target == null) return;

        // 마우스 입력
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -20f, 60f);

        // 카메라 위치 계산
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desiredPosition = target.position + rotation * offset;

        // 부드럽게 이동
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);

        // 항상 캐릭터 바라보기
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
