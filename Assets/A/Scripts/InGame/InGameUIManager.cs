using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGame
{
    public class InGameUIManager : MonoBehaviour, IActiveLink
    {
        [SerializeField] private InputDetail inputDetail;

        [Space(20)]
        [SerializeField] private Image runeBase;
        [SerializeField] private Image runeIcon;
        [SerializeField] private TextMeshProUGUI runeText;

        [Space(20)]
        [SerializeField] private Slider songSlider;
        [SerializeField] private Image songPlayer;

        [Space(20)]
        [SerializeField] private UIGameOverPopup gameOverPopup;
        private RectTransform songSliderRect;

        private const float UI_MOVE_DURATION = 1;

        private void Awake()
        {
            songSliderRect = songSlider.GetComponent<RectTransform>();
        }

        public void BounceHpBar(float duration)
        {
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

        public void Active()
        {
            runeBase.rectTransform.anchoredPosition = new Vector2(runeBase.rectTransform.anchoredPosition.x, 100);
            songSliderRect.anchoredPosition = new Vector2(-62.5f, songSliderRect.anchoredPosition.y);

            inputDetail.inputAction = Player.Instance.CheckInput;

            UpdateSongSlider(0);
            this.Invoke(() =>
            {
                songSliderRect.DOAnchorPosX(62.5f, UI_MOVE_DURATION);
                runeBase.rectTransform.DOAnchorPosY(-100, UI_MOVE_DURATION);
            }, 1);

            gameOverPopup.gameObject.SetActive(false);
            gameObject.SetActive(true);
        }

        public void DeActive()
        {
            runeBase.rectTransform.anchoredPosition = new Vector2(runeBase.rectTransform.anchoredPosition.x, -100);
            songSliderRect.anchoredPosition = new Vector2(62.5f, songSliderRect.anchoredPosition.y);

            inputDetail.inputAction = Player.Instance.CheckInput;

            UpdateSongSlider(0);
            songSliderRect.DOAnchorPosX(-62.5f, UI_MOVE_DURATION);
            runeBase.rectTransform.DOAnchorPosY(100, UI_MOVE_DURATION).OnComplete(() => { gameOverPopup.Popup(); });
        }
    }
}