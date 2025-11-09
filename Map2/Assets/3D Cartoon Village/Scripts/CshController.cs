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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("jump");
            rb.AddForce(Vector3.up * 200.0f);

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
