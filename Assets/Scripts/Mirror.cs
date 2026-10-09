using System;
using Unity.VisualScripting;
using UnityEngine;

public class Mirror : MonoBehaviour, ILightReceiver
{
    public bool isReflecting;
    private LineRenderer lineRenderer;

    Vector3 lightStart;
    Vector3 lightEnd;

    private Vector3[] startEndPoint;

    private void Start()
    {
        lineRenderer = GetComponentInChildren<LineRenderer>();

        lightStart = transform.position + Vector3.up;
        lightEnd = transform.position + transform.forward * 10 + Vector3.up;
    }

    private void Update()
    {
        UpdateLight();
        Check();
    }

    private void UpdateLight()
    {
        if (isReflecting)
        {
            lightStart = transform.position + Vector3.up;
            lightEnd = transform.position + transform.forward * 10 + Vector3.up;
            startEndPoint = new Vector3[2] { lightStart, lightEnd };

            lineRenderer.enabled = true;
            lineRenderer.SetPositions(startEndPoint);
        }
        else lineRenderer.enabled = false;
    }

    public void Check()
    {
        if (isReflecting)
        {
            if (Physics.Raycast(startEndPoint[0], transform.forward, out RaycastHit thing, 10))
            {
                lightEnd = thing.point;
                lineRenderer.SetPosition(1, thing.point);

                if (thing.collider.TryGetComponent(out ILightReceiver stuff)) stuff.LightReceived();
            }
        }
    }

    public void LightReceived()
    {
        isReflecting = true;
    }
}