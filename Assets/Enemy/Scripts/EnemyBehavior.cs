using System;
using UnityEngine;
using Random = UnityEngine.Random;

public enum EnemyState
{
   
    FollowTarget,
    Patrol,
    Attack
}

[RequireComponent( typeof(AIFollowTarget), typeof(AIPatrol))]
public class EnemyBehaviour : MonoBehaviour
{
    [field: SerializeField]
    public Transform target { get; private set; }

    public float speed { get; private set; } = 3.5f;

    [Header("Current State")]
    [SerializeField] private EnemyState state;

    [SerializeField] private float detectionRadius;

    [SerializeField] private AiBase[] states;

    private SphereCollider _collider;
   
    private bool _isDead;
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

    private readonly int Speed = Animator.StringToHash("Speed");
    private readonly int Die = Animator.StringToHash("Die");

    private void Awake()
    {
        states = GetComponents<AiBase>();
        ChangeState(state);
    }

    private void Start()
    {

      
    }

    private void Update()
    {
        switch (state)
        {
            
            case EnemyState.FollowTarget:
                UpdateFollowTarget();
                break;
            case EnemyState.Patrol:
                UpdatePatrol();
                break;
           
                
            case EnemyState.Attack:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void UpdateDeath()
    {
     
    }

    private void UpdatePatrol()
    {
        //For now, the conditions to change from patrol are the same as 
        //from Wander, so we are going to save code:
        UpdateWander();
    }

    private void UpdateFollowTarget()
    {
        if (PlayerIsOnRange(detectionRadius) && !_isDead) return;

        

        speed = 3.5f;
        
        var dice = Random.Range(0, 100);
        ChangeState(EnemyState.Patrol);

    }

    private void UpdateWander()
    {
        if (!PlayerIsOnRange(detectionRadius) && !_isDead) return;

       

        speed = 7f;
        ChangeState(EnemyState.FollowTarget);
     
    }

    private void ChangeState(EnemyState newState)
    {
        state = newState;

        for (int i = 0; i < states.Length; i++)
        {
            states[i].enabled = i == (int)state;

            /*if(i == (int)state)
                states[i].enabled = true;
            else   
                states[i].enabled = false;*/
        }
    }

    private bool PlayerIsOnRange(float detectionRadius)
    {
        if (!target) return false;
        var sqrDistance = (target.position - transform.position).sqrMagnitude;
        return sqrDistance <= Mathf.Pow(detectionRadius, 2);
    }
}

