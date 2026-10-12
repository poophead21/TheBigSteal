using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightStarter : MonoBehaviour
{
    private LineRenderer lightReflection;

    Vector3 lightStart;
    Vector3 lightEnd;

    private Vector3[] startEndPoint;


    private void Start()
    {
        lightReflection = GetComponentInChildren<LineRenderer>();
    }

    private void Update() 
    {
        UpdateLight();
        Check();
    }

    private void UpdateLight()
    {
        lightStart = transform.position + Vector3.up;
        lightEnd = transform.position + transform.forward * 10 + Vector3.up;
        startEndPoint = new Vector3[2] { lightStart, lightEnd };

        lightReflection.SetPositions(startEndPoint);
    }

    public void Check()
    {
        if (Physics.Raycast(startEndPoint[0], transform.forward, out RaycastHit thing, 10))
        {
            lightEnd = thing.point;
            lightReflection.SetPosition(1, thing.point);

            if (thing.collider.TryGetComponent(out ILightReceiver stuff)) stuff.LightReceived();
        }
    }
    

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(lightStart, lightEnd);
    }
}
