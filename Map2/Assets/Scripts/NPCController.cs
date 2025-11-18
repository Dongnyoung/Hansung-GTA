using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCController : MonoBehaviour
{
    public enum AIState { Wander, Flee, Down }
    AIState state = AIState.Wander;

    [Header("Wander Settings")]
    public float wanderRadius = 12f;
    public float changeTargetTime = 3f;
    float wanderTimer = 0f;

    [Header("Flee Settings")]
    public float fleeDistance = 8f;
    public float fleeDuration = 2f;
    float fleeTimer = 0f;

    [Header("HP Settings")]
    public float maxHP = 5f;
    float hp;

    [Header("Respawn")]
    public float respawnDelay = 3f;
    SpawnManager spawnManager;
    float spawnRadius;

    NavMeshAgent agent;
    Animator anim;

    public void Init(SpawnManager manager, float radius)
    {
        spawnManager = manager;
        spawnRadius = radius;
    }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        hp = maxHP;
    }

    void OnEnable()
    {
        hp = maxHP;
        state = AIState.Wander;
        wanderTimer = changeTargetTime;
        PickWanderDestination();
    }

    void Update()
    {
        // 속도 → movement 파라미터 (애니메이션 전환)
        if (anim) anim.SetFloat("movement", agent.velocity.magnitude);

        switch (state)
        {
            case AIState.Wander:
                HandleWander();
                break;

            case AIState.Flee:
                HandleFlee();
                break;

            case AIState.Down:
                break;
        }
    }

    // ------------------ WANDER ------------------
    void HandleWander()
    {
        wanderTimer += Time.deltaTime;
        if (wanderTimer >= changeTargetTime || agent.remainingDistance < 0.5f)
        {
            PickWanderDestination();
            wanderTimer = 0f;
        }
    }

    void PickWanderDestination()
    {
        Vector3 guess = transform.position + Random.insideUnitSphere * wanderRadius;
        if (NavMesh.SamplePosition(guess, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
    }

    // ------------------ FLEE ------------------
    void HandleFlee()
    {
        fleeTimer -= Time.deltaTime;
        if (fleeTimer <= 0f)
        {
            state = AIState.Wander;
        }
    }

    public void OnHit(Vector3 attackerPos, float damage = 1f)
    {
        if (state == AIState.Down) return;

        hp -= damage;
        if (hp <= 0f)
        {
            StartCoroutine(RespawnRoutine());
            return;
        }

        // 도망 목표 방향
        Vector3 away = (transform.position - attackerPos).normalized;
        away.y = 0f;
        Vector3 fleeTarget = transform.position + away * fleeDistance;

        if (NavMesh.SamplePosition(fleeTarget, out NavMeshHit hit, 3f, NavMesh.AllAreas))
            agent.SetDestination(hit.position);

        state = AIState.Flee;
        fleeTimer = fleeDuration;
    }

    // ------------------ RESPAWN ------------------
    IEnumerator RespawnRoutine()
    {
        state = AIState.Down;
        agent.ResetPath();
        agent.isStopped = true;

        // 잠깐 사라짐(비활성화)
        SetVisible(false);

        yield return new WaitForSeconds(respawnDelay);

        // 리스폰 위치
        // 리스폰 위치
        Vector3 pos = spawnManager
            ? spawnManager.GetFixedRespawnPoint()
            : transform.position; // 폴백




        transform.position = pos;
        agent.Warp(pos);

        hp = maxHP;
        agent.isStopped = false;
        state = AIState.Wander;

        SetVisible(true);
        PickWanderDestination();
    }

    void SetVisible(bool enable)
    {
        foreach (var r in GetComponentsInChildren<Renderer>())
            r.enabled = enable;
        foreach (var c in GetComponentsInChildren<Collider>())
            c.enabled = enable;
    }

    // 총알 충돌 감지
    /*
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
            OnHit(other.transform.position, 1);
    }*/
}
