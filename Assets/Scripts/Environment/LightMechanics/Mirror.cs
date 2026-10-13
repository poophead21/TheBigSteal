using UnityEngine;

public class Mirror : LightEmitter, ILightReceiver
{
    public void LightReceived()
    {
        isEmitting = true;
    }

    public void NoLightReceived()
    {
        isEmitting = false;
    }
}