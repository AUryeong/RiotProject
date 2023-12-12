using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class LobbyUISetting : LobbyUIActiveLink
    {
        [SerializeField] private Image background;
        [SerializeField] private Image settingPopup;
        
        [SerializeField] private Button exitButton;

        [SerializeField] private Slider syncSlider;
        
        [SerializeField] private Toggle buttonToggle;

        protected override void Awake()
        {
            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(DeActive);
        }

        public override void Active()
        {
            base.Active();
            
            background.DOKill();
            background.color = background.color.GetFade(0f);
            background.DOFade(0.45f, 0.5f);

            settingPopup.rectTransform.DOKill();
            settingPopup.rectTransform.localScale = Vector3.zero;
            settingPopup.rectTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);

            syncSlider.value = SaveManager.Instance.GameData.beatSync;
            buttonToggle.isOn = SaveManager.Instance.GameData.isButton;
        }

        public override void DeActive()
        {
            base.DeActive();
            
            background.DOKill(true);
            background.DOFade(0, 0.5f).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });

            settingPopup.rectTransform.DOKill(true);
            settingPopup.rectTransform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);

            SaveManager.Instance.GameData.beatSync = syncSlider.value;
            SaveManager.Instance.GameData.isButton = buttonToggle.isOn;
        }
    }
}