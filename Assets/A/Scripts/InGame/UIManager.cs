using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image hpBar;
    [SerializeField] private Image runeIcon;
    [SerializeField] private TextMeshProUGUI runeText;

    public void UpdateHpBar(float fillAmount)
    {
        hpBar.DOFillAmount(fillAmount, 0.2f);
    }

    public void UpdateRune(int rune)
    {
        runeIcon.rectTransform.DOPunchScale(Vector3.one * 0.4f, 0.2f);
        runeText.text = rune.ToString();
    }
}