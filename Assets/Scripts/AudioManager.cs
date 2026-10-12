using UnityEngine;

public class AudioManager : MonoBehaviour
{ 
    private AudioSource audioSource;
    public AudioClip[] audioClips;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();   
    }

    public void GetAudioClip(int clip)
    {
        foreach (AudioClip audioClip in audioClips)
        {
            if (audioClip == audioSource.clip)
            {
                audioSource.PlayOneShot(audioSource.clip);
            }
        }
    }
    
}
