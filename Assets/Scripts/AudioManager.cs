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
    [SerializeField] public List<AudioClip> clips = new();
    [Space]
    [SerializeField] AudioMixer mainMixer;
    [SerializeField] AudioMixerGroup masterGroup;
    [Space]
    [SerializeField] AudioSource source;

    [Header("Runtime")]
    public float Volume { get; private set; } = MaxVolume;
    public AudioSource bgmPlayer;
    public bool playing;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        Volume = MaxVolume;

        InitBgmPlayer();
        if (bgmPlayer != null)
            FadeVolume(bgmPlayer, Volume, 0.02f);
    }
    void OnApplicationQuit() => SetSavedVolume(Volume);

    public static void Play(AudioClip clip, float volume)
    {
        if (clip == null || Instance == null || Instance.source == null)
            return;

        AudioSource source = Instantiate(Instance.source);
        source.clip = clip;
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
        if (clip == null || Instance.bgmPlayer == null)
            yield break;

        if (clip == Instance.bgmPlayer.clip && Instance.bgmPlayer.isPlaying)
            yield break;

        FadeVolume(Instance.bgmPlayer, 0, transitionTime / 2);
        yield return new WaitForSeconds(transitionTime / 2);
        Instance.bgmPlayer.clip = clip;
        Instance.bgmPlayer.Play();
        Instance.playing = true;
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
        if (source == null)
        {
            Debug.LogError("AudioManager has no AudioSource prefab assigned.");
            return;
        }

        bgmPlayer = Instantiate(source, transform);
        bgmPlayer.outputAudioMixerGroup = masterGroup;
        bgmPlayer.loop = true;
        bgmPlayer.volume = Volume;

        AudioClip initialClip = GetClip(0);
        if (initialClip == null)
        {
            Debug.LogWarning("AudioManager has no BGM clips assigned.");
            playing = false;
            return;
        }

        bgmPlayer.clip = initialClip;
        bgmPlayer.Play();

        playing = true;
    }

    public AudioClip GetClip(int index)
    {
        if (clips == null || index < 0 || index >= clips.Count)
            return null;

        return clips[index];
    }
}
