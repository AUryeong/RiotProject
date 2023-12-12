using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGame
{
    public class InGameUIManager : MonoBehaviour
    {
        [SerializeField] private InputDetail inputDetail;

        [Space(20)]
        [SerializeField] private RectTransform hpBarList;
        [SerializeField] private Image[] hpBarBases;
        [SerializeField] private Image[] hpBars;

        [Space(20)]
        [SerializeField] private Image runeBase;
        [SerializeField] private Image runeIcon;
        [SerializeField] private TextMeshProUGUI runeText;

        [Space(20)]
        [SerializeField] private Slider songSlider;
        [SerializeField] private Image songPlayer;
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
            songPlayer.rectTransform.DOKill(true);
            songPlayer.rectTransform.DOScale(Vector3.one * 1.15f, duration).SetLoops(2, LoopType.Yoyo);
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
            foreach (var hpBar in hpBars)
                hpBar.color = TileManager.Instance.stageTileData.uiColor;
            
            hpBarList.anchoredPosition = new Vector2(-50, hpBarList.anchoredPosition.y);
            runeBase.rectTransform.anchoredPosition = new Vector2(runeBase.rectTransform.anchoredPosition.x, 100);
            songSliderRect.anchoredPosition = new Vector2(-62.5f, songSliderRect.anchoredPosition.y);

            inputDetail.inputAction = Player.Instance.CheckInput;

            UpdateSongSlider(0);
            this.Invoke(() =>
            {
                hpBarList.DOAnchorPosX(-100f, UI_MOVE_DURATION);
                songSliderRect.DOAnchorPosX(62.5f, UI_MOVE_DURATION);
                runeBase.rectTransform.DOAnchorPosY(-100, UI_MOVE_DURATION);
            }, 1);
        }
    }
}