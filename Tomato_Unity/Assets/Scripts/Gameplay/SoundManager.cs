using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField]
    private AudioMixerGroup musicMixerGroup;

    [SerializeField]
    private AudioMixerGroup sfxMixerGroup;

    [SerializeField]
    private List<SoundClip> musicTracks;

    [SerializeField]
    private List<SoundClip> sfxClips;

    private SoundClip trackPlaying;
    private SoundClip trackFading;
    private SoundClip sfxPlaying;



    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        foreach (var track in this.musicTracks)
        {
            track.audioSource = this.gameObject.AddComponent<AudioSource>();
            track.audioSource.clip = track.clip;
            track.audioSource.volume = track.volume;
            track.audioSource.pitch = Random.Range(track.pitchLower, track.pitchUpper);
            track.audioSource.loop = track.loop;
            track.audioSource.outputAudioMixerGroup = this.musicMixerGroup;
        }

        foreach (var clip in this.sfxClips)
        {
            clip.audioSource = this.gameObject.AddComponent<AudioSource>();
            clip.audioSource.clip = clip.clip;
            clip.audioSource.volume = clip.volume;
            clip.audioSource.pitch = Random.Range(clip.pitchLower, clip.pitchUpper);
            clip.audioSource.loop = clip.loop;
            clip.audioSource.outputAudioMixerGroup = this.sfxMixerGroup;
        }

        //play initial track
        this.PlayMusicTrack("helpme");
    }

    public void PlayMusicTrack(string title)
    {
        var track = this.musicTracks.Find(track => track.title == title);

        if (null == track)
        {
            Debug.Log("Sound track not found: " + title);
            return;
        }

        track.audioSource.Play();

        if (null != this.trackPlaying)
        {
            this.trackPlaying.audioSource.Stop();
        }

        this.trackPlaying = track;
    }

    public void StopMusicTrack(string title)
    {
        var track = this.musicTracks.Find(track => track.title == title);

        if (null == track)
        {
            Debug.Log("Sound track not found: " + title);
            return;
        }
        this.trackPlaying = null;
        track.audioSource.Stop();
    }

    public void PlaySoundEffect(string title)
    {
        var track = this.sfxClips.Find(track => track.title == title);

        if (null == track)
        {
            Debug.Log("Sound track not found: " + title);
            return;
        }

        track.audioSource.Play();
    }

    public void StopSoundEffect(string title)
    {
        var track = this.sfxClips.Find(track => track.title == title);

        if (null == track)
        {
            Debug.Log("Sound track not found: " + title);
            return;
        }

        track.audioSource.Stop();
    }
}

[System.Serializable]
public class SoundClip
{
    public AudioClip clip;

    [HideInInspector]
    public AudioSource audioSource;

    public string title;

    [Range(0f, 1f)]
    public float volume = 1.0f;

    [Range(0.1f, 3f)]
    public float pitchLower = 1.0f;
    [Range(0.1f, 3f)]
    public float pitchUpper = 1.0f;

    public bool loop = true;
}