using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class LobbyUISetting : MonoBehaviour, IActiveLink
    {
        [SerializeField] private Image background;
        [SerializeField] private Image settingPopup;
        
        [SerializeField] private Button exitButton;

        [SerializeField] private Slider syncSlider;

        private void Awake()
        {
            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(DeActive);
        }

        public void Active()
        {
            gameObject.SetActive(true);

            background.DOKill();
            background.color = background.color.GetFade(0f);
            background.DOFade(0.45f, 0.5f);

            settingPopup.rectTransform.DOKill();
            settingPopup.rectTransform.localScale = Vector3.zero;
            settingPopup.rectTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);

            syncSlider.value = SaveManager.Instance.GameData.beatSync;
        }

        public void DeActive()
        {
            background.DOKill(true);
            background.DOFade(0, 0.5f).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });

            settingPopup.rectTransform.DOKill(true);
            settingPopup.rectTransform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);

            SaveManager.Instance.GameData.beatSync = syncSlider.value;
        }
    }
}