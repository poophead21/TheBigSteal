using UnityEngine;

public class DelaySound : MonoBehaviour
{
    public AudioSource myAudioSource;
    public float delayInSeconds = 2.0f;

    void Start()
    {
        // Plays the assigned audio clip after 2 seconds
        myAudioSource.PlayDelayed(delayInSeconds);
    }
}
