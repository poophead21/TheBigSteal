using System;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Patrol,
    Seek,
    FollowTarget,
    Attack,
    Death
}
public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] public Transform target;
    
    public float speed { get; private set; } = 3.5f;
    [SerializeField] private float waterSpeed = 2f;
    private float chaseSpeed = 7f;

    [SerializeField] public float hearingRadius;
    private SphereCollider _colliderHearing;

    public Animator _animator;
   
    private NavMeshAgent _navMeshAgent;
    
    [SerializeField] private EnemyState state;
    [SerializeField] private AiBase[] states;
    
    private bool _isDead = false;
    public bool IsDead 
    {  
        get 
        { 
            return _isDead; 
        } 
        set 
        { 
            _isDead = value; 
        } 
    }
    
    private readonly int Speed = Animator.StringToHash("speed");
    
    private float seekTimer = 0f;
    private float endSeekTimer = 4.5f;
    private bool isSeeking = false;
    

    [SerializeField] private float viewAngle;
    [SerializeField] private float viewRadius;

    [SerializeField] private float attackRadius;


    private float looseSightTimer = 0;
    [SerializeField] private float timeToLooseSight;
    private bool hasLineOfSight = false;
    
    private void Awake()
    {
        states = GetComponents<AiBase>();
        ChangeState(state);
    }
    
    void Start()
    {
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        
        _colliderHearing = gameObject.AddComponent<SphereCollider>();
        _colliderHearing.radius = hearingRadius;
        _colliderHearing.isTrigger = true;
        
       
        
        _animator.SetFloat(Speed, speed);
    }

    void Update()
    { 
        _animator.SetFloat(Speed, _navMeshAgent.speed);
        switch (state)
        {
            case EnemyState.Patrol:
                UpdatePatrol();
                break;
            case EnemyState.Seek:
                UpdateSeek();
                break;
            case EnemyState.FollowTarget:
                UpdateFollowTarget();
                break;
            case EnemyState.Attack:
                UpdateAttack();
                break;
            case EnemyState.Death:
                UpdateDeath();
                break;
        }
    }

    private void UpdatePatrol()
    {
        
        if (CanSeePlayer() && !_isDead)
        {
            ChangeState(EnemyState.FollowTarget);
        }
        
        if (CanAttackPlayer() && !_isDead)
        {
            ChangeState(EnemyState.Attack);
        }
        
        
       
        
       
        
        if (_isDead)
        {
            ChangeState(EnemyState.Death);
            speed = 0;
            return;
        }
        
    }
    private void UpdateSeek()
    {
       

        if (!CanSeePlayer() && !_isDead && !isSeeking)
        {
            isSeeking = true;
        }
        
        if (CanSeePlayer() && !_isDead)
        {
            ChangeState(EnemyState.FollowTarget);
        }
        
        if (isSeeking)
        {
            seekTimer  += Time.deltaTime;
            if (seekTimer >= endSeekTimer)
            {
                seekTimer = 0;
                isSeeking = false;
                ChangeState(EnemyState.Patrol);
            }
            return;
        }
        
        if (_isDead)
        {
            ChangeState(EnemyState.Death);
            speed = 0;
            return;
        }
        
    }
    private void UpdateFollowTarget()
    {
        _navMeshAgent.speed = chaseSpeed;
        
        
        if (_isDead)
        {
            ChangeState(EnemyState.Death);
            speed = 0;
            return;
        }
        if (CanAttackPlayer() && !_isDead)
        {
            ChangeState(EnemyState.Attack);
        }
        
      
        
        if (CanSeePlayer() && !_isDead) return;

       

    }

    private void UpdateAttack()
    {
        
        
        if (CanAttackPlayer() && !_isDead) return;
        
        if (_isDead)
        {
            ChangeState(EnemyState.Death);
            speed = 0;
            return;
        }

        if (!CanAttackPlayer() && !_isDead)
        {
            ChangeState(EnemyState.FollowTarget);
        }
    }

    private void UpdateDeath()
    {
        _animator.SetBool("isDead", true);
    }

    private void ChangeState(EnemyState newState)
    {
        state = newState;

        for (int i = 0; i < states.Length; i++)
        {
            if(i == (int)state)
                states[i].enabled = true;
            else
                states[i].enabled = false;
        }
    }
    
   
    

    private bool CanSeePlayer()
    {
        if(!target) return false;
        
        Vector3 enemyToPlayerDirection = (target.position - transform.position).normalized;
        
        float angle = Vector3.Angle(transform.forward, enemyToPlayerDirection);
        
        if (angle > viewAngle * 0.5f) return false; //if player isnt even in the FOV
        
        if (Vector3.Distance(transform.position, target.position) > viewRadius) return false;

        if (Physics.Raycast(transform.position + Vector3.up, enemyToPlayerDirection, out RaycastHit hit, viewRadius))
        {
            hasLineOfSight = hit.transform == target;
        }

        if (hasLineOfSight)
        {
            looseSightTimer = 0f;
            return true;
        }
        else
        {
            looseSightTimer += Time.deltaTime;
        }

        if (looseSightTimer < timeToLooseSight) // if enemy still looking for player
        {
            return true;
        }

        return false; //enemy went over end search timer
    }

    private bool CanAttackPlayer()
    {
        if(!target) return false;

        return Vector3.Distance(transform.position, target.position) <= attackRadius;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Water"))
        {
            _navMeshAgent.speed = waterSpeed;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Water"))
        {
            _navMeshAgent.speed = speed;
        }
    }
    void OnDrawGizmosSelected()
    {
        float angle = 30.0f;
        float rayRange = 10.0f;
        float halfFOV = angle / 2.0f;
        float coneDirection = 0;

        Quaternion upRayRotation = Quaternion.AngleAxis(-halfFOV + coneDirection, Vector3.forward);
        Quaternion downRayRotation = Quaternion.AngleAxis(halfFOV + coneDirection, Vector3.forward);

        Vector3 upRayDirection = upRayRotation * transform.right * rayRange;
        Vector3 downRayDirection = downRayRotation * transform.right * rayRange;

        Gizmos.DrawRay(transform.position, upRayDirection);
        Gizmos.DrawRay(transform.position, downRayDirection);
        Gizmos.DrawLine(transform.position + downRayDirection, transform.position + upRayDirection);
    }   
}
