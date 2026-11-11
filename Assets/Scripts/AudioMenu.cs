using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AudioMenu : MonoBehaviour
{
    [SerializeField] Button toggleButton;
    [SerializeField] Slider volumeSlider;

    private void Start()
    {
        InitButton();
        InitSlider();
    }

    void InitButton()
    {

    }
    void InitSlider()
    {
        volumeSlider.minValue = AudioManager.MinVolume;
        volumeSlider.maxValue = AudioManager.MaxVolume;
        volumeSlider.onValueChanged.AddListener(AudioManager.SetVolume);
    }
}
