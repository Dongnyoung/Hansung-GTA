using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCWander : MonoBehaviour
{
    [Header("Wander")]
    [SerializeField] float wanderRadius = 10f;       // 배회 반경
    [SerializeField] float changeTargetTime = 3f;    // 목표 재설정 주기(초)
    [SerializeField] float arriveThreshold = 0.4f;   // 도착 판정 거리
    [SerializeField] int sampleTries = 20;         // NavMesh 샘플 재시도 횟수

    [Header("Stuck Handling")]
    [SerializeField] float stuckSpeed = 0.05f;       // 이 속도 이하이면 멈췄다고 봄
    [SerializeField] float stuckTime = 1.5f;        // 이 시간 이상 멈춰 있으면 재경로

    [Header("Animation (옵션)")]
    [SerializeField] Animator animator;              // 비워두면 자식에서 찾아줌
    [SerializeField] string speedParam = "Speed";    // Animator float 파라미터명

    NavMeshAgent agent;
    float timer;
    float stuckTimer;
    int speedHash;

    // Spawner에서 파라미터 주입하고 싶을 때 호출(선택)
    public void Init(float radius, float changeSec)
    {
        wanderRadius = radius;
        changeTargetTime = changeSec;
    }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        speedHash = Animator.StringToHash(speedParam);

        // 권장 기본값(프리팹에서 이미 세팅했으면 생략 가능)
        if (agent.stoppingDistance < 0.05f) agent.stoppingDistance = 0.1f;
        agent.autoBraking = true;
    }

    void OnEnable()
    {
        timer = changeTargetTime; // 바로 한 번 찍도록
        TrySetRandomDestinationImmediate();
    }

    void Update()
    {
        if (!agent.isOnNavMesh) return;

        // 애니메이션 연동(옵션)
        if (animator) animator.SetFloat(speedHash, agent.velocity.magnitude);

        timer += Time.deltaTime;

        // 1) 주기적으로 목적지 갱신
        if (timer >= changeTargetTime)
        {
            PickNewDestination();
            timer = 0f;
        }

        // 2) 도착했으면 즉시 새 목적지
        if (!agent.pathPending && agent.remainingDistance <= arriveThreshold)
        {
            PickNewDestination();
            timer = 0f;
        }

        // 3) 정지(막힘) 감지 → 재경로
        if (agent.velocity.sqrMagnitude <= stuckSpeed * stuckSpeed)
            stuckTimer += Time.deltaTime;
        else
            stuckTimer = 0f;

        if (stuckTimer >= stuckTime)
        {
            PickNewDestination();
            stuckTimer = 0f;
            timer = 0f;
        }
    }

    void TrySetRandomDestinationImmediate()
    {
        // 스폰 직후 첫 목적지 보장
        PickNewDestination();
    }

    void PickNewDestination()
    {
        // 반경 내 임의 위치를 여러 번 시도해서 NavMesh 위 점을 확보
        for (int i = 0; i < sampleTries; i++)
        {
            Vector3 guess = transform.position + Random.insideUnitSphere * wanderRadius;
            // 수평만 이동하도록 Y는 신경 안 씀(NavMesh가 높이를 정해줌)
            if (NavMesh.SamplePosition(guess, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                agent.isStopped = false;
                agent.SetDestination(hit.position);
                return;
            }
        }
        // 실패하면 다음 프레임에 다시 시도(아무 것도 안 함)
    }
}
