using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Audio Tracks")]
    [SerializeField] private AudioClip chaseMusicClip;
    [SerializeField] private AudioClip sneakMusicClip;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    [FormerlySerializedAs("maxVolume")]
    [Range(0f, 1f)]
    [SerializeField] private float chaseMaxVolume = 0.7f;
    [Range(0f, 1f)]
    [SerializeField] private float sneakMaxVolume = 0.7f;

    private AudioSource sneakAudioSource;
    private AudioSource chaseAudioSource;
    private Coroutine chaseSourceCoroutine;
    private Coroutine sneakSourceCoroutine;
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

        AudioSource[] audioSources = GetComponents<AudioSource>();
        
        chaseAudioSource = audioSources[0];
        chaseAudioSource.loop = true;
        chaseAudioSource.playOnAwake = false;
        chaseAudioSource.clip = chaseMusicClip;
        chaseAudioSource.volume = 0f;
        
        sneakAudioSource = audioSources[1];
        sneakAudioSource.loop = true;
        sneakAudioSource.playOnAwake = false;
        sneakAudioSource.clip = sneakMusicClip;
        sneakAudioSource.volume = sneakMaxVolume;
        sneakAudioSource.Play();
    }

    public void ReportChaseState(bool isChasing)
    {
        if (isChasing)
        {
            chasingEnemiesCount++;
            if (chasingEnemiesCount == 1)
            {
                print("START CHASE MUSIC");
                // First enemy started chasing -> Fade in chase music
                StartFade(chaseAudioSource, chaseMaxVolume);
                StartFade(sneakAudioSource, 0f);
            }
        }
        else
        {
            chasingEnemiesCount = Mathf.Max(0, chasingEnemiesCount - 1);
            if (chasingEnemiesCount == 0)
            {
                print("START SNEAK MUSIC");
                // All enemies stopped chasing -> Fade out music
                StartFade(chaseAudioSource, 0f);
                StartFade(sneakAudioSource, sneakMaxVolume);
            }
        }
    }

    private void StartFade(AudioSource source, float targetVolume)
    {
        if (source == chaseAudioSource)
        {
            if (chaseSourceCoroutine != null) StopCoroutine(chaseSourceCoroutine);
            chaseSourceCoroutine = StartCoroutine(FadeMusic(source, targetVolume));
        }
        else
        {
            if(sneakSourceCoroutine != null) StopCoroutine(sneakSourceCoroutine);
            sneakSourceCoroutine = StartCoroutine(FadeMusic(source, targetVolume));
        }
    }

    private IEnumerator FadeMusic(AudioSource source, float targetVolume)
    {
        if (targetVolume > 0f && !source.isPlaying)
        {
            source.Play();
        }

        float startVolume = source.volume;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, targetVolume, timer / fadeDuration);
            yield return null;
        }

        source.volume = targetVolume;

        if (targetVolume == 0f)
        {
            source.Stop();
        }
    }
}