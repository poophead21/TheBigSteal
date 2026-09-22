using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyPatrol))]
[RequireComponent(typeof(FieldOfView))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float patrolSpeed = 3.5f;

    [Header("Investigation Settings")]
    [Tooltip("How long the enemy WAITS looking around at its spot before moving to the last known position.")]
    [SerializeField] private float hesitationDelay = 1.5f;

    [Tooltip("EXACT TOTAL TIME (in seconds) the enemy will spend searching/investigating after losing sight before returning to patrol.")]
    [SerializeField] private float investigationDuration = 6.0f;

    [Header("Rotation Settings")]
    [SerializeField] private float turnSpeed = 8f;

    [Header("Animation Reference")]
    [SerializeField] private Animator anim;

    private EnemyPatrol patrolScript;
    private FieldOfView fovScript;
    private NavMeshAgent agent;

    private Vector3 lastKnownPosition;
    private bool hasLastKnownPos = false;
    private float investigationTimer = 0f;

    private void Awake()
    {
        patrolScript = GetComponent<EnemyPatrol>();
        fovScript = GetComponent<FieldOfView>();
        agent = GetComponent<NavMeshAgent>();

        // Auto-assign Animator from children if not set manually in Inspector
        if (anim == null)
            anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        // 1. DIRECT SIGHT: Player spotted
        if (fovScript.canSeePlayer && fovScript.playerRef != null)
        {
            patrolScript.enabled = false;

            // Reset timers while player is actively seen
            investigationTimer = 0f;

            // Store last known position
            lastKnownPosition = fovScript.playerRef.transform.position;
            hasLastKnownPos = true;

            // Look toward player
            LookAtTarget(lastKnownPosition);

            if (fovScript.isPlayerDetected)
            {
                agent.isStopped = false;
                agent.speed = chaseSpeed;
                agent.SetDestination(lastKnownPosition);
            }
            else
            {
                // Stand still and face player while alert timer fills up
                agent.isStopped = true;
            }
        }
        // 2. LOST SIGHT: Wait hesitationDelay, then go to last known position
        else if (hasLastKnownPos)
        {
            patrolScript.enabled = false;

            // Master timer tracking total time since sight was lost
            investigationTimer += Time.deltaTime;

            // FIXED DURATION EXPIRED -> Return to patrol
            if (investigationTimer >= investigationDuration)
            {
                AbandonSearchAndPatrol();
                return;
            }

            // Always turn towards the last known spot when standing
            LookAtTarget(lastKnownPosition);

            // STAGE A: Hesitation / Delay Period (Enemy stands still & inspects)
            if (investigationTimer < hesitationDelay)
            {
                agent.isStopped = true;
            }
            // STAGE B: Moving to last known position
            else
            {
                // Check if arrived at destination
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    agent.isStopped = true; // Arrived -> wait out remaining time
                }
                else
                {
                    agent.isStopped = false;
                    agent.speed = chaseSpeed;
                    agent.SetDestination(lastKnownPosition);
                }
            }
        }
        // 3. PATROL STATE: Normal patrolling
        else
        {
            if (!patrolScript.enabled)
            {
                agent.speed = patrolSpeed;
                patrolScript.enabled = true;
            }
        }

        // Drive animation states cleanly
        UpdateAnimations();
    }

    private void UpdateAnimations()
    {
        if (anim == null || agent == null) return;

        // 1. Check if the agent has a valid path and is meant to be moving
        bool hasPathToFollow = agent.hasPath && !agent.isStopped && agent.remainingDistance > agent.stoppingDistance;

        // 2. Multi-fallback velocity check (prevents isWalking staying false on low acceleration)
        bool isMovingByVelocity = agent.velocity.sqrMagnitude > 0.01f || agent.desiredVelocity.sqrMagnitude > 0.01f;

        bool isMoving = hasPathToFollow && isMovingByVelocity;

        // Inspecting is true when the enemy has a point of interest (hesitating or searching) but isn't moving
        bool isInspecting = hasLastKnownPos && !isMoving;

        // Send bools to Animator
        anim.SetBool("isWalking", isMoving);
        anim.SetBool("isInspecting", isInspecting);
    }

    private void LookAtTarget(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position);
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
        }
    }

    private void AbandonSearchAndPatrol()
    {
        hasLastKnownPos = false;
        investigationTimer = 0f;
        agent.speed = patrolSpeed;
        patrolScript.enabled = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (hasLastKnownPos)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(lastKnownPosition, 0.5f);
            Gizmos.DrawLine(transform.position, lastKnownPosition);
        }
    }
}