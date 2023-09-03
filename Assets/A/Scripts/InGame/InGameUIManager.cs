using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    
    [SerializeField] private Image hpBar;
    [SerializeField] private Image runeIcon;
    [SerializeField] private TextMeshProUGUI runeText;

    private void Start()
    {
        Active(GameManager.Instance.isGaming);
    }

    public void Active(bool isOn)
    {
        canvas.gameObject.SetActive(isOn);
    }
    

    public void UpdateHpBar(float fillAmount)
    {
        hpBar.DOFillAmount(fillAmount, 0.2f);
    }

    public void UpdateRune(int rune)
    {
        runeIcon.rectTransform.DOKill(true);
        runeIcon.rectTransform.DOPunchScale(Vector3.one * 0.4f, 0.2f);
        runeText.text = rune.ToString();
    }
}