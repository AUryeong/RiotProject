using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private Image hpBar;

    public void UpdateHpBar(float fillAmount)
    {
        hpBar.DOFillAmount(fillAmount, 0.2f);
    }
}