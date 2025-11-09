using UnityEngine;
using UnityEngine.AI;

public class NPCSpawner : MonoBehaviour
{
    public GameObject[] npcPrefabs;
    public float spawnRadius = 20f;
    public float spawnInterval = 2f;
    public float wanderRadius = 10f;
    public float changeTargetTime = 3f;
    public int npcCount = 10; // 최대 생성할 NPC 수

    private int currentNPCCount = 0;

    void Start()
    {
        // 2초마다 SpawnNPC 반복 실행
        InvokeRepeating(nameof(SpawnNPC), 0f, spawnInterval);
    }

    void SpawnNPC()
    {
        // 이미 10명 이상이면 더 이상 생성하지 않음
        if (currentNPCCount >= npcCount)
        {
            CancelInvoke(nameof(SpawnNPC)); // 자동으로 생성 중단
            return;
        }

        Vector3 spawnPos = transform.position + new Vector3(
            Random.Range(-spawnRadius, spawnRadius),
            30f,
            Random.Range(-spawnRadius, spawnRadius)
        );

        if (Physics.Raycast(spawnPos, Vector3.down, out RaycastHit hit, 100f))
        {
            int index = Random.Range(0, npcPrefabs.Length);
            GameObject npc = Instantiate(npcPrefabs[index], hit.point, Quaternion.identity);

            // 이동 AI 추가
            NavMeshAgent agent = npc.GetComponent<NavMeshAgent>();
            if (agent == null)
                agent = npc.AddComponent<NavMeshAgent>();

            npc.AddComponent<NPCWander>();

            currentNPCCount++;
        }
    }
}
