using UnityEngine;
using UnityEngine.AI;

public class AiBase : MonoBehaviour
{
    protected EnemyBehaviour _enemyBehaviour;
    protected NavMeshAgent _navMeshAgent;
    [SerializeField] protected float breakingDistance;

    protected virtual void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _enemyBehaviour = GetComponent<EnemyBehaviour>();
        _navMeshAgent.stoppingDistance = breakingDistance;
    }

    protected virtual void OnEnable()
    {
        _navMeshAgent.ResetPath();
        _navMeshAgent.stoppingDistance = breakingDistance;
        _navMeshAgent.speed = _enemyBehaviour.speed;
    }
}
