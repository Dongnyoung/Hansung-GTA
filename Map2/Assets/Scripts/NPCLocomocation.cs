using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCLocomotion : MonoBehaviour
{
    public string speedParam = "movement";
    Animator anim;
    NavMeshAgent agent;
    int speedHash;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        speedHash = Animator.StringToHash(speedParam);
        if (anim) anim.applyRootMotion = false;
    }

    void Update()
    {
        if (!anim) return;
        anim.SetFloat(speedHash, agent.velocity.magnitude);
    }
}
