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
    public Camera mainCamera;
    public Transform anchor;

    public List<Transform> slides = new();
    public Vector2 offsetFromCameraCenter;

    Queue<Transform> transitionQueue = new();
    int currentIndex;

    private void Start()
    {
        mainCamera = Camera.main;
        currentIndex = 0;

        RequestTransition(1);
        RequestTransition(1);
        RequestTransition(1);
        RequestTransition(1);
        RequestTransition(1);
        RequestTransition(1);

        CheckForTransition().Forget();
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
        if (currentIndex <= 0)
            currentIndex = slides.Count - 1;

        transitionQueue.Enqueue(slides[currentIndex]);
    }
    async Task Transition(Transform point, float transitionTime)
    {
        anchor.DOMove(-point.position + (Vector3)offsetFromCameraCenter, transitionTime);
        await anchor.DOScale(new Vector3(0.8f, 0.8f, 1), transitionTime / 2).AsyncWaitForCompletion();
        await anchor.DOScale(new Vector3(1, 1, 1), transitionTime / 2).AsyncWaitForCompletion();
    }
}
