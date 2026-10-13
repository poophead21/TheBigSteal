using UnityEngine;

public class LightEmitter : MonoBehaviour
{
    public bool isEmitting = true;
    public float height = 0.0f;
    
    private const float MaxDistance = 100f;
    
    private LineRenderer lineRenderer;
    private ILightReceiver hitReceiver;

    Vector3 lightStart;
    Vector3 lightEnd;

    private void Start()
    {
        lineRenderer = GetComponentInChildren<LineRenderer>();
        UpdateLight();
    }

    private void Update() 
    {
        UpdateLight();
    }
    
    private void UpdateLight()
    {
        if (isEmitting) UpdateLightOn();
        else UpdateLightOff();
    }

    private void UpdateLightOn()
    {
        lineRenderer.enabled = true;
        lightStart = transform.position + height * Vector3.up;
        lightEnd = transform.position + transform.forward * MaxDistance + height * Vector3.up;
        
        if (Physics.Raycast(lightStart, transform.forward, out RaycastHit hit, MaxDistance))
        {
            lightEnd = hit.point;

            if (hit.collider.TryGetComponent(out ILightReceiver lightReceiver))
            {
                lightReceiver.LightReceived();
                hitReceiver = lightReceiver;
            }
            else if (hitReceiver != null)
            {
                hitReceiver.NoLightReceived();
                hitReceiver = null;
            }
        } else if (hitReceiver != null)
        {
            hitReceiver.NoLightReceived();
            hitReceiver = null;
        }
        lineRenderer.SetPositions(new Vector3[2]{lightStart, lightEnd});
    }

    private void UpdateLightOff()
    {
        lineRenderer.enabled = false;
        if (hitReceiver != null)
        {
            hitReceiver.NoLightReceived();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(lightStart, lightEnd);
    }
}
