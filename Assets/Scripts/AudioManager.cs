using DG.Tweening;
using System.Collections;
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
    public bool playing;

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

    public static void Play(AudioClip clip, float volume)
    {
        AudioSource source = Instantiate(Instance.source);
        source.outputAudioMixerGroup = Instance.masterGroup;
        source.volume = volume;
        source.Play();
        Destroy(source.gameObject, clip.length);
    }
    public static void SetVolume(float volume)
    {
        Instance.mainMixer.SetFloat(MasterVolumeKey, volume);
        Instance.Volume = volume;
    }

    // BGM
    public static void ToggleBgm(float transitionTime) => Instance.StartCoroutine(ToggleBgmCR(transitionTime));
    static IEnumerator ToggleBgmCR(float transitionTime)
    {
        if (Instance.playing)
        {
            FadeVolume(Instance.bgmPlayer, 0, transitionTime);
            yield return new WaitForSeconds(transitionTime);
            Instance.bgmPlayer.Pause();
            Instance.playing = false;
        }
        else
        {
            FadeVolume(Instance.bgmPlayer, Instance.Volume, transitionTime);
            yield return new WaitForSeconds(transitionTime);
            Instance.bgmPlayer.UnPause();
            Instance.playing = true;
        }
    }

    public static void SwitchBgm(AudioClip clip, float transitionTime) => Instance.StartCoroutine(SwitchBgmCR(clip, transitionTime));
    static IEnumerator SwitchBgmCR(AudioClip clip, float transitionTime)
    {
        FadeVolume(Instance.bgmPlayer, 0, transitionTime / 2);
        yield return new WaitForSeconds(transitionTime / 2);
        Instance.bgmPlayer.clip = clip;
        FadeVolume(Instance.bgmPlayer, Instance.Volume, transitionTime / 2);
        yield return new WaitForSeconds(transitionTime / 2);
    }

    // Helpers
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

    // Save / Load
    void SetSavedVolume(float volume) => PlayerPrefs.SetFloat(MasterVolumeKey, volume);
    float GetSavedVolume() => PlayerPrefs.GetFloat(MasterVolumeKey);

    void InitBgmPlayer()
    {
        bgmPlayer = Instantiate(source, transform);
        bgmPlayer.outputAudioMixerGroup = masterGroup;
        bgmPlayer.loop = true;
        bgmPlayer.volume = Volume;
        bgmPlayer.Play();

        playing = true;
    }
}