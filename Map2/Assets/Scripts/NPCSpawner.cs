using UnityEngine;
using UnityEngine.AI;

public class NPCSpawner : MonoBehaviour
{
    public GameObject[] npcPrefabs;
    public float spawnRadius = 20f;
    public float spawnInterval = 2f;
    public float wanderRadius = 10f;
    public float changeTargetTime = 3f;

    void Start()
    {
        InvokeRepeating(nameof(SpawnNPC), 0f, spawnInterval);
    }

    void SpawnNPC()
    {
        Vector3 spawnPos = transform.position + new Vector3(
            Random.Range(-spawnRadius, spawnRadius),
            30f,
            Random.Range(-spawnRadius, spawnRadius)
        );

        if (Physics.Raycast(spawnPos, Vector3.down, out RaycastHit hit, 100f))
        {
            int index = Random.Range(0, npcPrefabs.Length);
            GameObject npc = Instantiate(npcPrefabs[index], hit.point, Quaternion.identity);

            // 이동 AI 붙여주기
            NavMeshAgent agent = npc.GetComponent<NavMeshAgent>();
            if (agent == null)
                agent = npc.AddComponent<NavMeshAgent>();

            npc.AddComponent<NPCWander>();
        }
    }
}
