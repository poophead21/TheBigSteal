using UnityEngine;

public class AudioManager : MonoBehaviour
{ 
    private AudioSource audioSource;
    public AudioClip[] audioClips;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();   
    }

    public void GetAudioClip(int clipIndex)
    {
        foreach (AudioClip audioClip in audioClips)
        {
            if (audioClip.name == audioClips[clipIndex].name)
            {
                audioSource.PlayOneShot(audioClips[clipIndex]);
            }
        }
    }
    
}
