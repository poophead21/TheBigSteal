using UnityEngine;

public class LightSensor : MonoBehaviour, ILightReceiver
{
    [SerializeField] private GameObject door;
    private Door doorScript;
    
    void Start()
    {
        doorScript  = door.GetComponent<Door>();
    }
    
    public void LightReceived(Vector3 incomingDirection)
    {
        doorScript.OpenDoor();
    }

    public void NoLightReceived()
    {
        doorScript.CloseDoor();
    }
}
