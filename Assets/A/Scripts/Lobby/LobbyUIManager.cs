using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class LobbyUIManager : MonoBehaviour
    {
        [SerializeField] private Image background;
        
        [SerializeField] private Image runeIcon;
        [SerializeField] private TextMeshProUGUI runeText;
        
        [SerializeField] private Button startButton;

        public const float UI_MOVE_DURATION = 1;

        private void Awake()
        {
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(() => GameManager.Instance.ActiveSceneLink(SceneLinkType.InGame));
        }

        public void ActiveSetting()
        {
            background.gameObject.SetActive(true);
            background.color = background.color.GetFade(0.3f);
            
            runeText.text = SaveManager.Instance.GameData.rune.ToString();

            runeIcon.rectTransform.anchoredPosition = new Vector2(100, runeIcon.rectTransform.anchoredPosition.y);
            runeIcon.rectTransform.DOAnchorPosX(-100, UI_MOVE_DURATION);
            
            startButton.image.rectTransform.anchoredPosition = new Vector2(startButton.image.rectTransform.anchoredPosition.x, -1000);
            startButton.image.rectTransform.DOAnchorPosY(0, UI_MOVE_DURATION);
        }
        
        public void DeActiveSetting()
        {
            background.color = background.color.GetFade(0.3f);
            background.DOFade(0, UI_MOVE_DURATION).OnComplete(() => gameObject.SetActive(false));
            
            runeIcon.rectTransform.anchoredPosition = new Vector2(-100, runeIcon.rectTransform.anchoredPosition.y);
            runeIcon.rectTransform.DOAnchorPosX(100, UI_MOVE_DURATION);
            
            startButton.image.rectTransform.anchoredPosition = Vector2.zero;
            startButton.image.rectTransform.DOAnchorPosY(-1000, UI_MOVE_DURATION);
        }
    }
}