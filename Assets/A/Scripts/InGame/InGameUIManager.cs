using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGame
{
    public class InGameUIManager : MonoBehaviour
    {
        [SerializeField] private Image hpBarBase;
        [SerializeField] private Image hpBar;

        [SerializeField] private Image runeIcon;
        [SerializeField] private TextMeshProUGUI runeText;

        public const float UI_MOVE_DURATION = 1;

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

        public void ActiveSetting()
        {
            hpBarBase.rectTransform.anchoredPosition = new Vector2(hpBarBase.rectTransform.anchoredPosition.x, 152);
            runeIcon.rectTransform.anchoredPosition = new Vector2(100, runeIcon.rectTransform.anchoredPosition.y);
            this.Invoke(() =>
            {
                hpBarBase.rectTransform.DOAnchorPosY(-105.2f, UI_MOVE_DURATION);

                runeIcon.rectTransform.DOAnchorPosX(-100, UI_MOVE_DURATION);
            }, 1);
        }
    }
}