using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    bool open = false;

    private Animation _animation;
    private AudioManager audioManager;
    private GameObject _player;
    private PlayerController playerController;

    private void Start()
    {
        _player = GameObject.FindWithTag("Player");
        playerController = _player.GetComponent<PlayerController>();
        _animation = GetComponent<Animation>();
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
            playerController.isMovingForCutscene = false;
            Debug.Log("door entered");
            StartCoroutine(OpenAndLoadNextLevel(other.gameObject));
        }
    }

    private IEnumerator OpenAndLoadNextLevel(GameObject player)
    {
        // Disable player movement control
        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
        }

        OpenDoor();
        yield return new WaitForSeconds(0.5f);
        //playerController.isMoving = true;
        ManageScenes.Instance.LoadNextLevel();
    }
}