using UnityEngine;

public class LightSensor : MonoBehaviour, ILightReceiver
{
    [SerializeField] private GameObject door;
    
    private AudioManager audioManager;
    private Door doorScript;
    
    void Start()
    {
        doorScript  = door.GetComponent<Door>();
        audioManager = GameObject.Find("SoundManager").GetComponent<AudioManager>();
    }
    
    public void LightReceived(Vector3 incomingDirection)
    {
        audioManager.PlayAudioClip(Sound.PuzzleComplete);
        doorScript.OpenDoor();
    }

    public void NoLightReceived()
    {
        doorScript.CloseDoor();
    }
}
