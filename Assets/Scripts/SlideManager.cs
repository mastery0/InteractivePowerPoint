using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using System.Collections;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine.PlayerLoop;

public class SlideManager : MonoBehaviour
{
    private Actions inputs;

    public Camera mainCamera;
    public RectTransform anchor;

    public List<Transform> slides = new();
    public Vector2 offsetFromCameraCenter;

    Queue<Transform> transitionQueue = new();
    int currentIndex;

    private void Start()
    {
        mainCamera = Camera.main;
        currentIndex = 0;

        CheckForTransition().Forget();

        inputs = new Actions();
        inputs.Enable();
    }

    private void Update()
    {
        if (inputs.Switch.Forward.WasPressedThisFrame())
            RequestTransition(1);

        if (inputs.Switch.Back.WasPressedThisFrame())
            RequestTransition(-1);
    }

    async UniTask CheckForTransition()
    {
        while (true)
        {
            if (transitionQueue.Count > 0)
                await Transition(transitionQueue.Dequeue(), 1f);

            await UniTask.WaitForEndOfFrame();
        }

    }

    public void RequestTransition(int shift)
    {
        currentIndex += shift;

        if (currentIndex >= slides.Count)
            currentIndex = 0;
        if (currentIndex < 0)
            currentIndex = slides.Count - 1;

        transitionQueue.Enqueue(slides[currentIndex]);
    }
    async Task Transition(Transform point, float transitionTime)
    {
        AudioClip clip = AudioManager.Instance?.GetClip(currentIndex);
        if (clip != null)
            AudioManager.SwitchBgm(clip, 1f);

        anchor.DOAnchorPos(-point.GetComponent<RectTransform>().anchoredPosition + offsetFromCameraCenter, transitionTime);
        await anchor.DOScale(new Vector3(0.8f, 0.8f, 1), transitionTime / 2).AsyncWaitForCompletion();
        await anchor.DOScale(new Vector3(1, 1, 1), transitionTime / 2).AsyncWaitForCompletion();
    }
}
