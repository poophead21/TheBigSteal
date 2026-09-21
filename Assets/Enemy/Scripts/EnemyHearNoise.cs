using UnityEngine;
using UnityEngine.AI;

public class EnemyHearNoise : MonoBehaviour
{
    private NavMeshAgent agent;

    private Vector3 patrolPosition;

    private bool investigating;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        patrolPosition = transform.position;
    }

    private void Update()
    {
       
        if (investigating && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            investigating = false;

           
            agent.SetDestination(patrolPosition);
        }
    }

    public void HearNoise(Vector3 noisePosition)
    {
        investigating = true;

        agent.SetDestination(noisePosition);
    }
}