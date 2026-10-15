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
        float rightAngleDiff = Mathf.Abs(angle % 90.0f);
        isEmitting = rightAngleDiff < 1.0f ||  rightAngleDiff > 89.0f;
        
        // assuming 2-way reflecting mirror that goes from forward left to back right
        if (angle > -1.0f && angle < 1.0f) emitAngle = -90.0f;
        if (angle > 89.0f && angle < 91.0f)  emitAngle = 0.0f;
        if (angle > 179.0f || angle <  -179.0f) emitAngle = 90.0f;
        if (angle > -91.0f && angle < -89.0f)  emitAngle = 180.0f;
    }
}