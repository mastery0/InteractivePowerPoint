using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Scenemanager : MonoBehaviour
{
    public List<GameObject> slides;
    GameObject _currentSlide;
    public GameObject currentSlide { get { return _currentSlide; }
        set {
            _currentSlide = value;
            foreach (var slide in slides) {
                if (slide==_currentSlide)
                {
                    _currentSlide.SetActive(true);
                    continue;
                }
                slide.SetActive(false);
            }
        } 
    }

    [Tooltip("False for going back and true for going forward.")]
    public void ChangeSlide(bool state)
    {
        if ((!state && slides.IndexOf(currentSlide) - 1 < 0) || (state && slides.IndexOf(currentSlide) + 1 > slides.Count)) return;
        if(!state)currentSlide= slides[slides.IndexOf(currentSlide) - 1];
        else currentSlide = slides[slides.IndexOf(currentSlide) + 1];
    }
    public void Initialize()
    {
        currentSlide = slides[0];
    }
}
