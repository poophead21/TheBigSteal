using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    bool open = false;
    
    private Animation _animation;
    private AudioManager audioManager;

    private void Start()
    {
        _animation  = GetComponent<Animation>();
        audioManager = GameObject.Find("SoundManager").GetComponent<AudioManager>();
    }

    public void OpenDoor()
    {
        if (open) return;
        open = true;
        _animation.Play("DoorOpen");
        audioManager.PlayAudioClip(Sound.DoorOpen);
    }
    
    public void CloseDoor()
    {
        if (!open) return;
        open = false;
        _animation.Play("DoorClose");
        audioManager.PlayAudioClip(Sound.DoorClose);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("door entered");
            ManageScenes.Instance.LoadNextLevel();
        }
    }
}
