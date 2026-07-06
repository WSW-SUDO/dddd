using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("音源组件")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("音频资源")]
    public AudioClip schoolBGM;
    public AudioClip clickSFX;

    private bool bgmMuted = false;
    private bool sfxMuted = false;
    private float lastBGMVolume = 0.5f;
    private float lastSFXVolume = 0.5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupAudioSources();
            LoadMuteState();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SetupAudioSources()
    {
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }

        bgmSource.volume = lastBGMVolume;
        sfxSource.volume = lastSFXVolume;
    }

    private void LoadMuteState()
    {
        bgmMuted = PlayerPrefs.GetInt("BGMMuted", 0) == 1;
        sfxMuted = PlayerPrefs.GetInt("SFXMuted", 0) == 1;

        if (bgmMuted) bgmSource.volume = 0;
        if (sfxMuted) sfxSource.volume = 0;

        Debug.Log("AudioManager: BGM=" + (!bgmMuted) + ", SFX=" + (!sfxMuted));
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null || bgmSource == null) return;
        
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void PlayBGM()
    {
        if (schoolBGM != null) PlayBGM(schoolBGM);
    }

    public void ToggleBGM()
    {
        bgmMuted = !bgmMuted;
        
        if (bgmMuted)
        {
            lastBGMVolume = bgmSource.volume;
            bgmSource.volume = 0;
            bgmSource.Pause();
        }
        else
        {
            bgmSource.volume = lastBGMVolume;
            bgmSource.UnPause();
        }

        PlayerPrefs.SetInt("BGMMuted", bgmMuted ? 1 : 0);
        PlayerPrefs.Save();
        
        Debug.Log("BGM " + (bgmMuted ? "OFF" : "ON"));
    }

    public void ToggleSFX()
    {
        sfxMuted = !sfxMuted;
        
        if (sfxMuted)
        {
            lastSFXVolume = sfxSource.volume;
            sfxSource.volume = 0;
        }
        else
        {
            sfxSource.volume = lastSFXVolume;
        }

        PlayerPrefs.SetInt("SFXMuted", sfxMuted ? 1 : 0);
        PlayerPrefs.Save();
        
        Debug.Log("SFX " + (sfxMuted ? "OFF" : "ON"));
    }

    public void PlayClickSFX()
    {
        if (clickSFX != null && sfxSource != null && !sfxMuted)
        {
            sfxSource.PlayOneShot(clickSFX);
        }
    }

    public bool IsBGMMuted() { return bgmMuted; }
    public bool IsSFXMuted() { return sfxMuted; }
}
