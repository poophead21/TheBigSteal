using UnityEngine;

public class AiPatrol : AiBase
{
    [SerializeField] private Transform[] patrolPoint;

    private int _currentPatrolPointIndex;

    protected override void Awake()
    {
        base.Awake();
        _navMeshAgent.updateRotation = true;
        _navMeshAgent.isStopped = false;
        GoToNextPoint();
    }

    private void Update()
    {
        if (_navMeshAgent.remainingDistance < _navMeshAgent.stoppingDistance && !_navMeshAgent.pathPending)
            GoToNextPoint();
    }

    private void GoToNextPoint()
    {
        if (_currentPatrolPointIndex >= patrolPoint.Length) _currentPatrolPointIndex = 0;
        _navMeshAgent.SetDestination(patrolPoint[_currentPatrolPointIndex++].position);
    }
    
}
