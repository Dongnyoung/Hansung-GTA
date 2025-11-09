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

    [Header("Jump")]
    public float jumpForce = 5f;
    public float groundCheckDistance = 0.25f; // 바닥 감지 거리
    public LayerMask groundMask = ~0;         // 필요시 Ground 레이어로 제한

    private bool isGrounded;
    void Start()
    {
        currentRunningGaze = maxRunningGaze;
        isrunning = 1.0f;
        animator = GetComponent<Animator>();
        HP = maxHP;

        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; // 넘어짐 방지
    }

    void Update()
    {
        gameTime += Time.deltaTime;
        currentTime += Time.deltaTime;

        moveDirection = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) moveDirection += Vector3.forward;
        if (Input.GetKey(KeyCode.A)) moveDirection += Vector3.left;
        if (Input.GetKey(KeyCode.S)) moveDirection += Vector3.back;
        if (Input.GetKey(KeyCode.D)) moveDirection += Vector3.right;

        if (Input.GetKeyDown(KeyCode.LeftShift)) isrunning = 2.5f;
        if (Input.GetKeyUp(KeyCode.LeftShift)) isrunning = 1.0f;
        // --- 착지 체크 (Raycast) ---
        // 살짝 위에서 아래로 쏴서 바닥 감지 (캐릭터가 바닥과 겹칠 때 오검출 방지)
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, groundCheckDistance + 0.1f, groundMask);

        // --- 점프 ---
        // GetKeyDown + isGrounded 조건으로 "한 번만" 점프
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            animator.SetTrigger("jump");

            // 점프 직전 Y속도를 0으로 만들어 일관된 점프 높이 보장(선택)
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        moveDirection.Normalize();

        // 회전 처리
        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, 0.2f); // 회전 속도 약간 빠르게
        }

        animator.SetFloat("movement", moveDirection.magnitude * isrunning);

        // 기존 HP, 스킬 등 로직 그대로 유지
        /*
        if (Input.GetKeyDown(KeyCode.Space) && currentTime > skillCooltime)
        {
            currentTime = 0;
        }

        if (gameTime >= 20)
        {
            animator.SetTrigger("victory");
        }
        */
    }

    void FixedUpdate()
    {
        // Rigidbody 기반 이동 (충돌 처리)
        Vector3 velocity = moveDirection * moveSpeed * isrunning;
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }
}
