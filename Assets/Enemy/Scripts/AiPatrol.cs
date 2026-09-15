using UnityEngine;
using UnityEngine.AI;

public class AIPatrol : AiBase
{
    [SerializeField] private Transform[] patrolPoint;

    private int _currentPatrolPointIndex;

    protected override void Awake()
    {
        base.Awake();
        GoToNextPoint();
    }

    private void Update()
    {
        if (agent.remainingDistance < agent.stoppingDistance && !agent.pathPending)
            GoToNextPoint();
    }

    private void GoToNextPoint()
    {
        if (_currentPatrolPointIndex >= patrolPoint.Length) _currentPatrolPointIndex = 0;
        agent.SetDestination(patrolPoint[_currentPatrolPointIndex++].position);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        //Do something when this state si entered
        Debug.Log("Entered Patrol State");
    }
}
