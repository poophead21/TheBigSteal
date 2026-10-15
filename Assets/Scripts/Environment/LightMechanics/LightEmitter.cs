using UnityEngine;

public class LightEmitter : MonoBehaviour
{
    public bool isEmitting = true;
    protected float emitAngle = 0.0f;
    public float height = 0.0f;
    
    private const float MaxDistance = 100f;
    
    private LineRenderer lineRenderer;
    private ILightReceiver lightReceiver;

    Vector3 lightStart;
    Vector3 lightEnd;
    
    private void Start()
    {
        lineRenderer = GetComponentInChildren<LineRenderer>();
        lineRenderer.enabled = false;
        UpdateLight();
    }

    protected virtual void Update() 
    {
        if (transform.hasChanged)
            UpdateLight();
    }
    
    private void UpdateLight()
    {
        if (isEmitting) UpdateLightOn();
        else UpdateLightOff();
    }

    private void UpdateLightOn()
    {
        Vector3 emitDirection = Quaternion.Euler(0.0f, emitAngle, 0.0f) * transform.forward;
        
        lineRenderer.enabled = true;
        lightStart = transform.position + height * Vector3.up;
        lightEnd = transform.position + emitDirection * MaxDistance + height * Vector3.up;
        
        if (Physics.Raycast(lightStart, emitDirection, out RaycastHit hit, MaxDistance))
        {
            lightEnd = hit.point;

            if (hit.collider.TryGetComponent(out ILightReceiver newLightReceiver))
            {
                if (lightReceiver != newLightReceiver)
                {
                    if (lightReceiver != null)
                        lightReceiver.NoLightReceived();
                    lightReceiver = newLightReceiver;
                    lightReceiver.LightReceived(emitDirection);
                }
            }
            else if (lightReceiver != null)
            {
                lightReceiver.NoLightReceived();
                lightReceiver = null;
            }
        } else if (lightReceiver != null)
        {
            lightReceiver.NoLightReceived();
            lightReceiver = null;
        }
        lineRenderer.SetPositions(new Vector3[2]{lightStart, lightEnd});
    }

    private void UpdateLightOff()
    {
        lineRenderer.enabled = false;
        if (lightReceiver != null)
        {
            lightReceiver.NoLightReceived();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(lightStart, lightEnd);
    }
}
