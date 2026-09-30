using System;
using UnityEngine;

public class Door : MonoBehaviour
{
    private Animation _animation;

    private void Start()
    {
        _animation  = GetComponent<Animation>();
    }

    private void OnEnable()
    {
        GameManager.Instance.OnCPActivated += OpenDoor;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnCPActivated -= OpenDoor;
    }

    private void OpenDoor()
    {
        _animation.Play("DoorOpen");
    }
}
