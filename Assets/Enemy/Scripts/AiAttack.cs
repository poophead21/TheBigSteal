using System;
using UnityEngine;

public class AiAttack : AiBase
{
    private float timer = 0;
    [SerializeField] private float attackTimer = 2f;
    
    void Start()
    {
        _navMeshAgent.updateRotation = true;
        _navMeshAgent.isStopped = true;
    }

    private void Update()
    {
        timer  += Time.deltaTime;
        if (timer < attackTimer) return;
        
        timer = 0;
        _enemyBehaviour._animator.SetTrigger("punch");
    }
}
