using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIButtonColorChanger : MonoBehaviour
{
    [SerializeField] private Image[] whiteColorImages;
    [SerializeField] private Image[] lightColorImages;
    [SerializeField] private Image[] darkColorImages;

    private RectTransform rectTransform;
    public RectTransform RectTransform => rectTransform ? rectTransform : (rectTransform = GetComponent<RectTransform>());

    public void Apply(Color lightColor, Color darkColor)
    {
        foreach (var image in lightColorImages)
            image.color = lightColor;

        foreach (var image in darkColorImages)
            image.color = darkColor;
    }

    public void Apply(float alpha)
    {
        foreach (var image in lightColorImages)
        {
            image.DOKill();
            image.color = image.color.GetAlpha(alpha);
        }

        foreach (var image in darkColorImages)
        {
            image.DOKill();
            image.color = image.color.GetAlpha(alpha);
        }
        
        foreach (var image in whiteColorImages)
        {
            image.DOKill();
            image.color = image.color.GetAlpha(alpha);
        }
    }

    public void ApplyFade(float duration, Color lightColor, Color darkColor, float delay = 0)
    {
        foreach (var image in lightColorImages)
        {
            image.DOKill(true);
            image.DOColor(lightColor, duration).SetDelay(delay);
        }

        foreach (var image in darkColorImages)
        {
            image.DOKill(true);
            image.DOColor(darkColor, duration).SetDelay(delay);
        }
    }

    public void ApplyFade(float duration, float fade, float delay = 0)
    {
        foreach (var image in whiteColorImages)
        {
            image.DOKill(true);
            image.DOFade(fade, duration).SetDelay(delay);
        }
        
        foreach (var image in lightColorImages)
        {
            image.DOKill(true);
            image.DOFade(fade, duration).SetDelay(delay);
        }

        foreach (var image in darkColorImages)
        {
            image.DOKill(true);
            image.DOFade(fade, duration).SetDelay(delay);
        }
    }
}
