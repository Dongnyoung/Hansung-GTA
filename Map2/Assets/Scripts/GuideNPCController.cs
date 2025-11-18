using UnityEngine;

public class GuideNPCController : MonoBehaviour
{
    public float followDistance = 2f; // NPC가 유지할 거리
    public float moveSpeed = 2f;

    private Transform player;
    private Animator anim;
    public bool isStopped = false;

    void Start()
    {
        // Tag 이름이 "Player"인 오브젝트를 찾음
        GameObject p = GameObject.FindGameObjectWithTag("Player");

        if (p != null)
            player = p.transform;

        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (player == null) return;
        if (isStopped) { anim.SetFloat("movement", 0); return; }

        FollowPlayer();
    }

    void FollowPlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > followDistance)
        {
            Vector3 dir = (player.position - transform.position).normalized;

            transform.position += dir * moveSpeed * Time.deltaTime;
            transform.forward = dir;

            // movement 값이 0.5보다 크면 walk 애니메이션 재생됨
            anim.SetFloat("movement", moveSpeed);
        }
        else
        {
            anim.SetFloat("movement", 0f);
        }
    }
}
