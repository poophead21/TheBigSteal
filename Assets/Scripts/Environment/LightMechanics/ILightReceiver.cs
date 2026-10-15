using UnityEngine;

public interface ILightReceiver
{
    public void LightReceived(Vector3 incomingDirection);
    public void NoLightReceived();
}