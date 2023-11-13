using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace InGame
{
    public class InGameUIManager : MonoBehaviour
    {
        [SerializeField] private RectTransform hpBarList;
        [SerializeField] private Image[] hpBarBases;
        [SerializeField] private Image[] hpBars;

        [Space(20)]
        [SerializeField] private Image runeBase;
        [SerializeField] private Image runeIcon;
        [SerializeField] private TextMeshProUGUI runeText;

        [Space(20)]
        [SerializeField] private Slider songSlider;
        private RectTransform songSliderRect;

        private const float UI_MOVE_DURATION = 1;

        private void Awake()
        {
            songSliderRect = songSlider.GetComponent<RectTransform>();
        }

        public void BounceHpBar(float duration)
        {
            foreach (var hpBarBase in hpBarBases)
            {
                hpBarBase.rectTransform.DOKill(true);
                hpBarBase.rectTransform.DOScale(Vector3.one * 1.15f, duration).SetLoops(2, LoopType.Yoyo);
            }
        }

        public void UpdateHpBar(float fillAmount)
        {
            for (var i = 0; i < hpBars.Length; i++)
            {
                var hpBar = hpBars[i];
                float prevMultiplier = i / 3f;
                hpBar.DOFillAmount((fillAmount - prevMultiplier) * 3f, 0.2f);
            }
        }
        
        public void UpdateSongSlider(float fillAmount)
        {
            songSlider.value = fillAmount;
        }

        public void UpdateRune(int rune)
        {
            runeIcon.rectTransform.DOKill(true);
            runeIcon.rectTransform.DOPunchScale(Vector3.one * 0.4f, 0.2f);
            runeText.text = rune.ToString();
        }

        public void ActiveSetting()
        {
            hpBarList.anchoredPosition = new Vector2(hpBarList.anchoredPosition.x, 152);
            runeBase.rectTransform.anchoredPosition = new Vector2(runeBase.rectTransform.anchoredPosition.x, 100);
            songSliderRect.anchoredPosition = new Vector2(60, songSliderRect.anchoredPosition.y);
            UpdateSongSlider(0);
            this.Invoke(() =>
            {
                hpBarList.DOAnchorPosY(-90f, UI_MOVE_DURATION);
                songSliderRect.DOAnchorPosX(-50, UI_MOVE_DURATION);
                runeBase.rectTransform.DOAnchorPosY(-100, UI_MOVE_DURATION);
            }, 1);
        }
    }
}