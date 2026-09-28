using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightStarter : MonoBehaviour
{
    public bool isReflecting;
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
        Lighter();
    }
    private void Lighter()
    {
        lightStart = transform.position + Vector3.up;
        lightEnd = transform.position + transform.forward * 10 + Vector3.up;
        startEndPoint = new Vector3[2] { lightStart, lightEnd };

        lightReflection.SetPositions(startEndPoint);
        lightReflection.enabled = true;

        Mirror stuff;
        if (Physics.Raycast(startEndPoint[0], transform.forward, out RaycastHit thing, 10))
        {
            lightEnd = thing.point;
            startEndPoint[1] = lightEnd;

            if (thing.collider.gameObject.TryGetComponent(out stuff))
            {
                stuff.Toggle();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(lightStart, lightEnd);
    }
}
