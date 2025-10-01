using UnityEngine;
using UnityEngine.AI;

public class NPCWander : MonoBehaviour
{
    public float wanderRadius = 10f;  // How far NPC can wander
    public float wanderDelay = 3f;    // Time between new destinations

    private NavMeshAgent agent;
    private Animator animator;
    private float timer;

    private int footpathAreaMask;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // IMPORTANT: Check if the NavMeshAgent component exists
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component not found on the NPC.");
            enabled = false; // Disable the script if no agent is found
            return;
        }

        // Get the Area Mask from the NavMeshAgent component.
        footpathAreaMask = agent.areaMask;

        // --- FIX: Defer the first destination call slightly ---
        // Calling SetDestination in Start() can fail if the NavMesh isn't fully ready.
        // We'll let Update handle the first destination after the first timer tick.
        timer = 0f; // Set timer to 0 so Update() calls SetNewDestination immediately
    }

    void Update()
    {
        // --- FIX: Check if agent is valid/active before proceeding ---
        if (agent == null || !agent.isActiveAndEnabled)
        {
            // Agent is not ready, skip logic this frame
            return;
        }

        timer -= Time.deltaTime;

        // If idle too long (or reached destination), pick new destination
        // --- FIX: Added check for agent.isOnNavMesh to prevent errors on remainingDistance/hasPath ---
        if (agent.isOnNavMesh && timer <= 0f && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            SetNewDestination();
            timer = wanderDelay;
        }
        // Also call SetNewDestination if the timer is 0 for the first time, even if not at the destination
        else if (timer <= 0f && agent.isOnNavMesh)
        {
            SetNewDestination();
            timer = wanderDelay;
        }


        // Switch animation
        if (animator != null)
        {
            if (agent.isOnNavMesh && agent.velocity.magnitude > 0.1f)
            {
                animator.SetBool("isWalking", true);
            }
            else
            {
                animator.SetBool("isWalking", false);
            }
        }
    }

    void SetNewDestination()
    {
        // --- FIX: Check if agent is active and on the mesh before setting destination ---
        if (agent.isOnNavMesh)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, footpathAreaMask);
            agent.SetDestination(newPos);
        }
    }

    // Pick random point on NavMesh
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int areaMask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * dist;
        randomDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, dist, areaMask);

        return navHit.position;
    }
}