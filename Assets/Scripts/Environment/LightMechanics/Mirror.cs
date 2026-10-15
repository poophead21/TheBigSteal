using UnityEngine;

public class Mirror : LightEmitter, ILightReceiver
{
    private Vector3 incomingLightDirection;
    private bool isReceivingLight = false;
    
    private Mirror()
    {
        isEmitting = false;
    }

    protected override void Update()
    {
        if (transform.hasChanged && isReceivingLight)
        {
            UpdateReflection();
        }
        base.Update();
    }
    
    public void LightReceived(Vector3 incomingDirection)
    {
        isReceivingLight = true;
        incomingLightDirection = incomingDirection;
        UpdateReflection();
    }

    public void NoLightReceived()
    {
        isReceivingLight = false;
        isEmitting = false;
    }

    public void UpdateReflection()
    {
        float angle = Vector3.SignedAngle(incomingLightDirection, transform.forward, Vector3.up);
        isEmitting = true;
        
        // assuming one-way reflecting mirror that goes from forward left to back right with the reflective side on the
        // back-left
        if (angle > -1.0f && angle < 1.0f) emitAngle = -90.0f;
        else if (angle > -91.0f && angle < -89.0f)  emitAngle = 180.0f;
        else isEmitting = false;
    }
}