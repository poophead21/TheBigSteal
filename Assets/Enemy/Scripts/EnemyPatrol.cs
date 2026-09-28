using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    [Tooltip("List of transforms the enemy will patrol between.")]
    [SerializeField] private Transform[] waypoints;

    [Tooltip("How long to wait at each waypoint before moving to the next.")]
    [SerializeField] private float waitTimeAtWaypoint = 1.5f;

    private NavMeshAgent agent;
    private int currentWaypointIndex = 0;
    private bool isWaiting = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        // Resume navigation when script is re-enabled
        if (agent != null && waypoints != null && waypoints.Length > 0)
        {
            isWaiting = false;
            agent.isStopped = false;
            SetDestinationToCurrentWaypoint();
        }
    }

    private void OnDisable()
    {
        // Stop navigation cleanly when disabled by EnemyController
        if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }
    }

    private void Update()
    {
        if (waypoints == null || waypoints.Length == 0 || isWaiting)
            return;

        // FIXED: Wait until path is calculated AND agent has actual remaining distance
        if (!agent.pathPending && agent.hasPath && agent.remainingDistance <= agent.stoppingDistance)
        {
            StartCoroutine(WaitAtWaypoint());
        }
    }

    private void SetDestinationToCurrentWaypoint()
    {
        if (waypoints != null && waypoints.Length > currentWaypointIndex && waypoints[currentWaypointIndex] != null)
        {
            agent.isStopped = false;
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    private IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;

        yield return new WaitForSeconds(waitTimeAtWaypoint);

        // Advance to next waypoint index
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        SetDestinationToCurrentWaypoint();

        isWaiting = false;
    }

    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;
            Gizmos.DrawSphere(waypoints[i].position, 0.3f);

            int nextIndex = (i + 1) % waypoints.Length;
            if (waypoints[nextIndex] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[nextIndex].position);
            }
        }
    }
}