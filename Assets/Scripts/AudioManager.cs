using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public const string MasterVolumeKey = "Master";
    public const float MaxVolume = 1f;
    public const float MinVolume = 0.001f;

    [Header("Setup")]
    [SerializeField] public Dictionary<string, AudioClip> clips = new();
    [Space]
    [SerializeField] AudioMixer mainMixer;
    [SerializeField] AudioMixerGroup masterGroup;
    [Space]
    [SerializeField] AudioSource source;
    
    [Header("Runtime")]
    public float Volume { get; private set; }
    public AudioSource bgmPlayer;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        Volume = GetSavedVolume();

        InitBgmPlayer();
        FadeVolume(bgmPlayer, Volume, 0.02f);
    }
    void OnApplicationQuit() => SetSavedVolume(Volume);

    // Publics
    public static void Play(AudioClip clip, float volume)
    {
        AudioSource source = Instantiate(Instance.source);
        source.outputAudioMixerGroup = Instance.masterGroup;
        source.volume = volume;
        source.Play();
        Destroy(source.gameObject, clip.length);
    }
    public static void SetBgmClip(AudioClip clip) => Instance.bgmPlayer.clip = clip;
    public static void SetBgmVolume(float volume) => Instance.bgmPlayer.volume = volume;

    public static float VolumeToDb(float volume)
    {
        volume = Mathf.Clamp01(volume);
        return volume <= 0f ? -80f : Mathf.Log10(volume) * 20f;
    }
    public static void FadeVolume(AudioSource source, float endVolume, float transitionTime)
    {
        source.DOKill();
        DOTween.To(() => source.volume,
            volume => source.volume = volume,
            Mathf.Clamp01(endVolume),
            transitionTime);
    }

    // Privates
    float GetSavedVolume() => PlayerPrefs.GetFloat(MasterVolumeKey);
    void SetSavedVolume(float volume) => PlayerPrefs.SetFloat(MasterVolumeKey, volume);

    void InitBgmPlayer()
    {
        bgmPlayer = Instantiate(source, transform);
        bgmPlayer.outputAudioMixerGroup = masterGroup;
        bgmPlayer.loop = true;
        bgmPlayer.volume = 0f;
        bgmPlayer.Play();
    }
}