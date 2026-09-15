using UnityEngine;

public class AIFollowTarget : AiBase
{
    [SerializeField] private float updateTime = 0.2f;

    private float _timeCounter = 0;
    private void Update()
    {
        _timeCounter += Time.deltaTime;
        if (_timeCounter < updateTime) return;
        _timeCounter = 0;
        agent.SetDestination(enemyBehaviour.target.position);
    }
}
