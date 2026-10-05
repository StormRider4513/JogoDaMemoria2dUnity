using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Mixer")]
    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;

    [Header("Músicas de Fundo")]
    [SerializeField] private AudioClip bg4x4;
    [SerializeField] private AudioClip bg6x6;
    [SerializeField] private AudioClip bg8x8;
    [SerializeField] private AudioClip bgMenu;

    [Header("Efeitos Sonoros")]
    [SerializeField] private AudioClip flip;
    [SerializeField] private AudioClip unflip;
    [SerializeField] private AudioClip correct;
    [SerializeField] private AudioClip wrong;
    [SerializeField] private AudioClip click;

    [Header("Fade")]
    [SerializeField] private float musicFadeDuration = 1.5f;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    private const float MIN_VALUE = 0.0001f;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = true;
        musicSource.outputAudioMixerGroup = musicGroup;
        sfxSource.outputAudioMixerGroup = sfxGroup;

        LoadVolumes();
    }

    // ================= VOLUME =================

    private void ApplyMusicVolume(float value)
    {
        value = Mathf.Clamp(value, MIN_VALUE, 1f);
        mainMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20f);
    }

    private void ApplySFXVolume(float value)
    {
        value = Mathf.Clamp(value, MIN_VALUE, 1f);
        mainMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20f);
    }

    public void SetMusicVolume(float value)
    {
        DOTween.Kill("MusicFade");

        ApplyMusicVolume(value);
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        ApplySFXVolume(value);
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
    }

    private void LoadVolumes()
    {
        float music = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 1f);

        ApplyMusicVolume(music);
        ApplySFXVolume(sfx);
    }

    // ================= MÚSICA =================

    public void PlayMusicForMode(int gridSize)
    {
        AudioClip clip = gridSize switch
        {
            4 => bg4x4,
            6 => bg6x6,
            8 => bg8x8,
            0 => bgMenu,
            _ => bg4x4
        };

        if (clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.Play();

        FadeMusicIn();
    }

    public void StopMusic()
    {
        FadeMusicOut();
    }

    private void FadeMusicIn()
    {
        DOTween.Kill("MusicFade");

        mainMixer.SetFloat("MusicVolume", -80f);

        float target = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float targetDb = Mathf.Log10(Mathf.Clamp(target, MIN_VALUE, 1f)) * 20f;

        mainMixer
            .DOSetFloat("MusicVolume", targetDb, musicFadeDuration)
            .SetId("MusicFade");
    }

    private void FadeMusicOut()
    {
        DOTween.Kill("MusicFade");

        mainMixer
            .DOSetFloat("MusicVolume", -80f, musicFadeDuration)
            .SetId("MusicFade");
    }

    // ================= SFX =================

    public void PlayFlip() => sfxSource.PlayOneShot(flip);
    public void PlayUnflip() => sfxSource.PlayOneShot(unflip);
    public void PlayCorrect() => sfxSource.PlayOneShot(correct);
    public void PlayWrong() => sfxSource.PlayOneShot(wrong);
    public void PlayClick() => sfxSource.PlayOneShot(click);
}
