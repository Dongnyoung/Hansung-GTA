using UnityEngine;
using UnityEngine.AI;

public class SpawnManager : MonoBehaviour
{
    [Header("NPC Prefabs (여러 개)")]
    [SerializeField] GameObject[] npcPrefabs;

    [Header("Spawn Settings")]
    [SerializeField] int initialCount = 8;

    [Header("NPC 기본 이동 세팅(프리팹에 없을 때만 적용)")]
    [SerializeField] float defaultSpeed = 3.5f;
    [SerializeField] float defaultAngularSpeed = 120f;
    [SerializeField] float defaultAcceleration = 8f;

    [Header("Animation")]
    [SerializeField] RuntimeAnimatorController animatorController;

    [Header("Fixed Respawn Point")]
    [Tooltip("리스폰 기준점(선택). 비우면 Map 기반으로 임의 1회 선정해서 고정")]
    [SerializeField] Transform respawnAnchor;
    [SerializeField] float snapMaxDistance = 8f;

    // 캐시된 고정 리스폰 지점
    Vector3 fixedRespawnPoint;
    bool hasFixedPoint;

    [Header("Map-bounded Sampling (필수: map 지정)")]
    [Tooltip("맵(바닥/층 루트) 오브젝트 드래그 (Renderer/Collider/자식 Renderer로 Bounds 산출)")]
    [SerializeField] Transform map;                // ★ 인스펙터에서 반드시 지정
    [Tooltip("NavMesh.SamplePosition 반경 (실패 잦으면 키우기)")]
    [SerializeField] float sampleRadius = 35f;
    [Tooltip("Walkable만 쓰려면: 1 << NavMesh.GetAreaFromName(\"Walkable\")")]
    [SerializeField] int areaMask = NavMesh.AllAreas;

    void Start()
    {
        if (!ValidateMapAssigned()) return;

        EnsureFixedRespawnPoint();

        for (int i = 0; i < initialCount; i++)
            SpawnOne();
    }

    bool ValidateMapAssigned()
    {
        if (map != null) return true;
        Debug.LogError("[SpawnManager] map이 비어있습니다. 2floor(또는 바닥 루트) 오브젝트를 map에 드래그하세요.");
        return false;
    }

    void EnsureFixedRespawnPoint()
    {
        if (hasFixedPoint) return;

        // 1) Anchor 우선
        if (respawnAnchor &&
            NavMesh.SamplePosition(respawnAnchor.position, out var hit1, snapMaxDistance, areaMask))
        {
            fixedRespawnPoint = hit1.position;
            hasFixedPoint = true;
            return;
        }

        // 2) Anchor 없거나 실패 → Map 기준 랜덤 → Raycast → NavMesh 스냅
        fixedRespawnPoint = RandomPointOnMapNavMesh(map, sampleRadius, areaMask);
        hasFixedPoint = true;
    }

    public Vector3 GetFixedRespawnPoint()
    {
        if (!hasFixedPoint) EnsureFixedRespawnPoint();
        return fixedRespawnPoint;
    }

    public void SpawnOne()
    {
        if (npcPrefabs == null || npcPrefabs.Length == 0) return;
        if (!ValidateMapAssigned()) return;

        // Map 기준 랜덤 → 지면 Raycast → NavMesh.SamplePosition
        Vector3 spawnPos = RandomPointOnMapNavMesh(map, sampleRadius, areaMask);

        var prefab = npcPrefabs[Random.Range(0, npcPrefabs.Length)];
        var rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        GameObject npc = Instantiate(prefab, spawnPos, rot);

        // NavMeshAgent 보장 + 기본값
        var agent = npc.GetComponent<NavMeshAgent>();
        if (!agent) agent = npc.AddComponent<NavMeshAgent>();
        if (!agent.isOnNavMesh) agent.Warp(spawnPos);

        if (agent.speed <= 0.1f) agent.speed = defaultSpeed;
        if (agent.angularSpeed <= 0f) agent.angularSpeed = defaultAngularSpeed;
        if (agent.acceleration <= 0f) agent.acceleration = defaultAcceleration;
        if (agent.stoppingDistance < 0.05f) agent.stoppingDistance = 0.1f;

        // Animator 주입
        var anim = npc.GetComponentInChildren<Animator>();
        if (anim != null)
        {
            if (animatorController != null) anim.runtimeAnimatorController = animatorController;
            anim.applyRootMotion = false; // 이동은 Agent 담당
        }
        else
        {
            Debug.LogError($"[SpawnManager] {npc.name} 프리팹에 Animator가 없습니다. (Avatar 포함 필요)");
        }

        // 이동 속도 → 애니 파라미터 연동(필요 시)
        if (!npc.TryGetComponent<NPCLocomotion>(out var loco))
            loco = npc.AddComponent<NPCLocomotion>();
        // loco.speedParam = "movement"; // Animator 파라미터명이 "movement"면 주석 해제

        // AI 컨트롤러 보장
        var ctrl = npc.GetComponent<NPCController>();
        if (!ctrl) ctrl = npc.AddComponent<NPCController>();
        ctrl.Init(this, 0f);
    }

    /// <summary>
    /// Map Bounds 안에서: 랜덤 XZ → 위에서 아래 Raycast → NavMesh.SamplePosition 으로 스냅
    /// </summary>
    public Vector3 RandomPointOnMapNavMesh(Transform mapRoot, float radius, int mask, int attempts = 80)
    {
        // 1) Bounds 계산 (Renderer > Collider > 자식 Renderer 합산)
        Bounds b = GetMapBounds(mapRoot);

        // 2) Bounds 랜덤 → Raycast ↓ → 3) NavMesh 스냅
        for (int i = 0; i < attempts; i++)
        {
            float x = Random.Range(b.min.x, b.max.x);
            float z = Random.Range(b.min.z, b.max.z);

            Vector3 from = new Vector3(x, b.max.y + 20f, z);

            if (Physics.Raycast(from, Vector3.down, out RaycastHit hit, b.size.y + 40f))
            {
                if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, radius, mask))
                    return navHit.position;
            }
            else
            {
                // 맵이 얇거나 콜라이더가 끊긴 경우 대비
                Vector3 guess = new Vector3(x, b.center.y, z);
                if (NavMesh.SamplePosition(guess, out NavMeshHit navHit2, radius, mask))
                    return navHit2.position;
            }
        }

        // 실패 시: 고정 리스폰 포인트 또는 현재 위치
        return hasFixedPoint ? fixedRespawnPoint : transform.position;
    }

    Bounds GetMapBounds(Transform mapRoot)
    {
        // Renderer 우선
        var rend = mapRoot.GetComponent<Renderer>();
        if (rend != null) return rend.bounds;

        // Collider 다음
        var col = mapRoot.GetComponent<Collider>();
        if (col != null) return col.bounds;

        // 자식 Renderer 합산
        var rs = mapRoot.GetComponentsInChildren<Renderer>();
        if (rs.Length > 0)
        {
            Bounds b = rs[0].bounds;
            for (int i = 1; i < rs.Length; i++) b.Encapsulate(rs[i].bounds);
            return b;
        }

        // 안전 폴백(아예 Bounds가 없을 경우)
        Debug.LogWarning("[SpawnManager] map에 Renderer/Collider가 없습니다. 임시 Bounds 사용");
        return new Bounds(mapRoot.position, Vector3.one * 50f);
    }

    // 시각화: 고정 리스폰 지점 표시
    void OnDrawGizmosSelected()
    {
        if (hasFixedPoint)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(fixedRespawnPoint, 0.3f);
        }
        else if (respawnAnchor)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(respawnAnchor.position, 0.3f);
        }
    }
}
