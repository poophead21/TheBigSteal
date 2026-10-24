using System;
using System.Collections;
using UnityEngine;

public class Sandwich : Interactable
{
    private AudioManager audioManager;

    public void Start()
    {
        audioManager = GameObject.Find("SoundManager").GetComponent<AudioManager>();
    }

    public override bool CanInteract(GameObject interactor, InteractionType interactionType)
    {
        return true;
    }

    public override void Interact(GameObject interactor, InteractionType interactionType)
    {
        Debug.Log("Sandwich interacted");

        StartCoroutine(EndSequence());
    }

    private IEnumerator EndSequence()
    {
        // Temporarily hardcoded; should be replaced later.
        
        audioManager.PlayAudioClip(Sound.OpenVault1);
        yield return new WaitForSeconds(1.6f);
        audioManager.PlayAudioClip(Sound.OpenVault2);
        yield return new WaitForSeconds(2.8f);
        audioManager.PlayAudioClip(Sound.GetSandwich);
        yield return new WaitForSeconds(1.8f);
        ManageScenes.Instance.UnloadLevel();
        MusicManager.Instance.PlayWinMusic();
    }
    
}
