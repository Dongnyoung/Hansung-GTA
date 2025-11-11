using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CshController : MonoBehaviour
{
    float isrunning;
    public float HP;
    public float maxHP = 10;

    public float gameTime;
    public float currentTime;
    public float skillCooltime = 10f;
    public float currentRunningGaze;
    public float maxRunningGaze;
    public float moveSpeed = 2f;
    public Animator animator;

    private Rigidbody rb;
    private Vector3 moveDirection;

    [Header("Camera Reference")]
    public Transform playerCamera; // 인스펙터에서 카메라 연결

    [Header("Jump")]
    public float jumpForce = 5f;
    public float groundCheckDistance = 0.25f;
    public LayerMask groundMask = ~0;

    private bool isGrounded;

    void Start()
    {
        currentRunningGaze = maxRunningGaze;
        isrunning = 1.0f;
        animator = GetComponent<Animator>();
        HP = maxHP;

        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        gameTime += Time.deltaTime;
        currentTime += Time.deltaTime;

        // --- 입력 처리 ---
        float h = Input.GetAxisRaw("Horizontal"); // A/D 좌우
        float v = Input.GetAxisRaw("Vertical");   // W/S 앞뒤

        Vector3 forward = Vector3.zero;
        Vector3 right = Vector3.zero;

        if (playerCamera != null)
        {
            // 카메라 방향 기준으로 forward, right 벡터 계산 (y 축 제거)
            forward = playerCamera.forward;
            forward.y = 0;
            forward.Normalize();

            right = playerCamera.right;
            right.y = 0;
            right.Normalize();
        }
        else
        {
            // 카메라 연결 안하면 월드 기준
            forward = Vector3.forward;
            right = Vector3.right;
        }

        moveDirection = (forward * v + right * h).normalized;

        // --- 달리기 ---
        if (Input.GetKey(KeyCode.LeftShift)) isrunning = 2.5f;
        else isrunning = 1.0f;

        // --- 착지 체크 ---
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, groundCheckDistance + 0.1f, groundMask);

        // --- 점프 ---
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            animator.SetTrigger("jump");
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // --- 회전 ---
        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, 0.2f);
        }

        animator.SetFloat("movement", moveDirection.magnitude * isrunning);
    }

    void FixedUpdate()
    {
        // Rigidbody 기반 이동
        Vector3 velocity = moveDirection * moveSpeed * isrunning;
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }
}
