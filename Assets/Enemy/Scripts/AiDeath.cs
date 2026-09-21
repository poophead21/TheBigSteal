using UnityEngine;

public class AiDeath : AiBase
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _navMeshAgent.ResetPath();
    }
}
