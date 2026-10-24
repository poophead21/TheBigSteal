using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Sound
{
    WoodScrape, Footsteps, DoorOpen, DoorClose, PuzzleComplete, PlayerCaught, OpenVault1, OpenVault2, GetSandwich
}

[Serializable]
public struct AudioClipInfo
{
    public AudioClip clip;
    [Range(0f, 999f)] public float delay;
    [Range(0f, 1f)] public float volume;
}

public class AudioManager : MonoBehaviour
{
    [SerializeField] private Sound[] audioClipsKeys;
    [SerializeField] private AudioClipInfo[] audioClipsValues;
    
    private Dictionary<Sound, AudioClipInfo> audioClips;
    
    private static AudioManager _instance;

    private AudioSource _audioSource;
    
    private void Awake()
    {
        _instance = this;
        
        audioClips = new Dictionary<Sound, AudioClipInfo>();
        for (int i = 0; i < audioClipsKeys.Length; i++)
            audioClips.Add(audioClipsKeys[i], audioClipsValues[i]);
    }
    
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();   
    }

    // void Update()
    // {
    //     print(_instance._audioSource.isPlaying);
    // }

    public void PlayAudioClip(Sound clip)
    {
        if (!audioClips.ContainsKey(clip)) return;
        
        AudioClipInfo clipInfo = audioClips[clip];
        
        _instance._audioSource.volume = clipInfo.volume;
        
        if (clipInfo.delay > 0)
            StartCoroutine(PlayAudioClipDelayed(clipInfo.clip, clipInfo.delay));
        else
        {
            _instance._audioSource.PlayOneShot(clipInfo.clip);
        }
        print(_instance._audioSource.isPlaying);
    }

    private IEnumerator PlayAudioClipDelayed(AudioClip clip, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _instance._audioSource.PlayOneShot(clip);
    }

    public void StartPlayingAudioClip(Sound clip, bool force = false)
    {
        if (!audioClips.ContainsKey(clip)) return;

        AudioClipInfo clipInfo = audioClips[clip];
        if (force && _instance._audioSource.clip != clipInfo.clip)
        {
            _instance._audioSource.Stop();
            _instance._audioSource.clip = clipInfo.clip;
        }
        
        if (_instance._audioSource.isPlaying) return;
        
        _audioSource.volume = clipInfo.volume;
        
        if (clipInfo.delay > 0)
            StartCoroutine(StartPlayingDelayed(clipInfo.delay));
        else
            _instance._audioSource.Play();
    }
    
    private IEnumerator StartPlayingDelayed(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _instance._audioSource.Play();
    }
    
    public void StopPlayingAudioClip(Sound clip)
    {
        if (!audioClips.ContainsKey(clip)) return;
        
        if (_instance._audioSource.clip == audioClips[clip].clip)
            _instance._audioSource.Stop();
    }
}
