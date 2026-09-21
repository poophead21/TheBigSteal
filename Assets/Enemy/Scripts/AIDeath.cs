using UnityEngine;

public class AIDeath : AiBase
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        agent.ResetPath();
    }
}
