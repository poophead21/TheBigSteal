using UnityEngine;

public class AiSeek : AiBase
{
    private float rotationSpeed= 80f;
    void Start() 
    {
        _navMeshAgent.updateRotation = false;
        _navMeshAgent.ResetPath();
    }

    void Update() 
    {
        //rotate enemy
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}
