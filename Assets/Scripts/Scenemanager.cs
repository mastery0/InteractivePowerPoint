using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Scenemanager : MonoBehaviour
{
    public List<GameObject> slides;
    public GameObject currentSlide;

    [Tooltip("False for going back and true for going forward.")]
    public void ChangeSlide(bool state)
    {
        if ((!state && slides.IndexOf(currentSlide) - 1 < 0) || (state && slides.IndexOf(currentSlide) > slides.Count)) return;
        if(!state)currentSlide= slides[slides.IndexOf(currentSlide) - 1];
        else currentSlide = slides[slides.IndexOf(currentSlide) + 1];

        foreach (var slide in slides)
        {
            if (slide == currentSlide)
            {
                currentSlide.SetActive(true);
                continue;
            }
            slide.SetActive(false);
        }
    }

    [ContextMenu("Initialize")]
    public void Initialize()
    {
        currentSlide = slides[0];
        foreach (var slide in slides)
        {
            if (slide == currentSlide)
            {
                currentSlide.SetActive(true);
                continue;
            }
            slide.SetActive(false);
        }
    }
}
