using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSourceA;
    public AudioSource musicSourceB;
    public AudioSource sfxSource;

    [System.Serializable]
    public struct MusicTrack
    {
        public AudioClip clip;
        public bool useClipping;
        public float startTime;
        public float endTime;
    }

    [Header("Music")]
    public System.Collections.Generic.List<MusicTrack> musicPlaylist;
    [Range(0f, 1f)]
    public float musicVolume = 1f;
    public float fadeDuration = 2f;

    private MusicTrack? currentTrack;
    private bool isUsingSourceA = true;
    private System.Collections.IEnumerator fadeCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (musicPlaylist != null && musicPlaylist.Count > 0)
        {
            PlayRandomMusic();
        }
    }

    private void Update()
    {
        // Debug Input to test crossfade
        if (Input.GetKeyDown(KeyCode.M))
        {
            PlayRandomMusic();
        }

        AudioSource activeSource = isUsingSourceA ? musicSourceA : musicSourceB;

        if (activeSource != null)
        {
            // Only control volume if NOT fading (fading handles its own volume)
            if (fadeCoroutine == null)
            {
                activeSource.volume = musicVolume;
            }

            // Handle Clipping
            if (currentTrack.HasValue && currentTrack.Value.useClipping && activeSource.isPlaying)
            {
                if (activeSource.time >= currentTrack.Value.endTime)
                {
                    activeSource.time = currentTrack.Value.startTime;
                }
            }
        }
    }

    public void PlayRandomMusic()
    {
        if (musicPlaylist == null || musicPlaylist.Count == 0) return;
        
        int randomIndex = Random.Range(0, musicPlaylist.Count);
        PlayMusic(musicPlaylist[randomIndex]);
    }

    public void PlayMusic(MusicTrack track)
    {
        if (track.clip == null) return;
        
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = CrossfadeToMusic(track);
        StartCoroutine(fadeCoroutine);
    }

    private System.Collections.IEnumerator CrossfadeToMusic(MusicTrack track)
    {
        AudioSource activeSource = isUsingSourceA ? musicSourceA : musicSourceB;
        AudioSource nextSource = isUsingSourceA ? musicSourceB : musicSourceA;

        // Setup Next Source
        currentTrack = track;
        nextSource.clip = track.clip;
        nextSource.loop = true;
        nextSource.volume = 0f;
        
        if (track.useClipping)
        {
            nextSource.time = track.startTime;
        }
        else
        {
            nextSource.time = 0f;
        }

        nextSource.Play();

        // Crossfade
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;

            if (activeSource.isPlaying)
            {
                activeSource.volume = Mathf.Lerp(musicVolume, 0f, t);
            }
            nextSource.volume = Mathf.Lerp(0f, musicVolume, t);

            yield return null;
        }

        // Finalize
        if (activeSource.isPlaying)
        {
            activeSource.Stop();
            activeSource.volume = 0f;
        }
        nextSource.volume = musicVolume;

        // Swap Active Source Flag
        isUsingSourceA = !isUsingSourceA;
        
        fadeCoroutine = null;
    }

    // Overload for backward compatibility or direct clip playing
    public void PlayMusic(AudioClip clip)
    {
        MusicTrack tempTrack = new MusicTrack { clip = clip, useClipping = false };
        PlayMusic(tempTrack);
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        
        sfxSource.PlayOneShot(clip, volume);
    }

    public void PlayRandomSFX(AudioClip[] clips, float volume = 1f)
    {
        if (clips == null || clips.Length == 0) return;

        int randomIndex = Random.Range(0, clips.Length);
        sfxSource.PlayOneShot(clips[randomIndex], volume);
    }
}
