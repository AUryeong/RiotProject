using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonColorChanger : MonoBehaviour
{
    [SerializeField] private Image[] lightColorImages;
    [SerializeField] private Image[] darkColorImages;

    private RectTransform rectTransform;
    public RectTransform RectTransform
    {
        get
        {
            if (ReferenceEquals(rectTransform, null))
                rectTransform = GetComponent<RectTransform>();
            return rectTransform;
        }
    }

    public void Apply(Color lightColor, Color darkColor)
    {
        foreach (var image in lightColorImages)
            image.color = lightColor;

        foreach (var image in darkColorImages)
            image.color = darkColor;
    }

    public void ApplyFade(float duration, Color lightColor, Color darkColor)
    {
        foreach (var image in lightColorImages)
        {
            image.DOKill(true);
            image.DOColor(lightColor, duration);
        }

        foreach (var image in darkColorImages)
        {
            image.DOKill(true);
            image.DOColor(darkColor, duration);
        }
    }
}
