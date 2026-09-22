using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Audio Tracks")]
    [SerializeField] private AudioClip chaseMusicClip;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 1.0f;
    [Range(0f, 1f)]
    [SerializeField] private float maxVolume = 0.7f;

    private AudioSource audioSource;
    private Coroutine fadeCoroutine;
    private int chasingEnemiesCount = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.clip = chaseMusicClip;
        audioSource.volume = 0f;
    }

    public void ReportChaseState(bool isChasing)
    {
        if (isChasing)
        {
            chasingEnemiesCount++;
            if (chasingEnemiesCount == 1)
            {
                // First enemy started chasing -> Fade in chase music
                StartFade(maxVolume);
            }
        }
        else
        {
            chasingEnemiesCount = Mathf.Max(0, chasingEnemiesCount - 1);
            if (chasingEnemiesCount == 0)
            {
                // All enemies stopped chasing -> Fade out music
                StartFade(0f);
            }
        }
    }

    private void StartFade(float targetVolume)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeMusic(targetVolume));
    }

    private IEnumerator FadeMusic(float targetVolume)
    {
        if (targetVolume > 0f && !audioSource.isPlaying)
        {
            audioSource.Play();
        }

        float startVolume = audioSource.volume;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, timer / fadeDuration);
            yield return null;
        }

        audioSource.volume = targetVolume;

        if (targetVolume == 0f)
        {
            audioSource.Stop();
        }
    }
}