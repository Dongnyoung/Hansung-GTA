using UnityEngine;
using UnityEngine.AI;

public class NPCWander : MonoBehaviour
{
    public float wanderRadius = 10f;
    public float changeTargetTime = 3f;

    NavMeshAgent agent;
    float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = changeTargetTime;
        PickNewDestination();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= changeTargetTime)
        {
            PickNewDestination();
            timer = 0f;
        }
    }

    void PickNewDestination()
    {
        Vector3 randomDir = Random.insideUnitSphere * wanderRadius;
        randomDir += transform.position;

        if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
}
