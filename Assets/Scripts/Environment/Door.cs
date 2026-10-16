using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    bool open = false;
    
    private Animation _animation;

    private void Start()
    {
        _animation  = GetComponent<Animation>();
    }

    private void OnEnable()
    {
        //GameManager.Instance.OnCPActivated += OpenDoor;
    }

    private void OnDisable()
    {
        //GameManager.Instance.OnCPActivated -= OpenDoor;
    }

    public void OpenDoor()
    {
        if (open) return;
        open = true;
        _animation.Play("DoorOpen");
    }
    
    public void CloseDoor()
    {
        if (!open) return;
        open = false;
        _animation.Play("DoorClose");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigg entered");
        ManageScenes.Instance.LoadNextLevel();
    }
}
