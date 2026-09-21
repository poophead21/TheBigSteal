using System;
using UnityEngine;

public class AiFollowTarget : AiBase
{
    [SerializeField] private float updateTime = 0.2f;

    private float _timeCounter = 0;

    private void Start()
    {
        _navMeshAgent.updateRotation = true;
        _navMeshAgent.isStopped = false;
    }

    private void Update()
    {
        //every 0.2f refresh destination
        _timeCounter += Time.deltaTime;
        if (_timeCounter < updateTime) return;
        _timeCounter = 0;
        _navMeshAgent.SetDestination(_enemyBehaviour.target.position);
    }
}
