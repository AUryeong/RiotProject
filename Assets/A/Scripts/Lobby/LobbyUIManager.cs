using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class LobbyUIManager : MonoBehaviour, IActiveLink
    {
        [SerializeField] private Image background;

        [Header("Buttons")]
        [SerializeField] private Image downButtonWindow;

        [SerializeField] private Button[] downButtons;
        [SerializeField] private LobbyUIActiveLink[] downButtonPopup;

        public const float UI_MOVE_DURATION = 0.5f;
        public const float UI_BUTTON_MOVE_DURATION = 0.4f;

        private int lastDownIndex = -1;

        private void Awake()
        {
            for (var i = 0; i < downButtons.Length; i++)
            {
                var downButton = downButtons[i];
                int temp = i;

                downButton.onClick.RemoveAllListeners();
                downButton.onClick.AddListener(() => DownButtonClick(temp));
            }
        }

        private void DownButtonClick(int index)
        {
            if (lastDownIndex >= 0)
            {
                downButtonPopup[lastDownIndex].DeActive();
                downButtons[lastDownIndex].image.DOColor(Color.white, UI_BUTTON_MOVE_DURATION);
                if (downButtonPopup[lastDownIndex] == downButtonPopup[index])
                {
                    lastDownIndex = -1;
                    return;
                }
            }

            downButtons[index].image.DOColor(new Color(0.75f, 0.75f, 0.75f), UI_BUTTON_MOVE_DURATION);

            downButtonPopup[index].Active();
            lastDownIndex = index;
        }

        public void Active()
        {
            foreach (var lobbyActiveLink in downButtonPopup)
                lobbyActiveLink.gameObject.SetActive(false);

            DownButtonClick(1);

            background.DOKill();
            downButtonWindow.rectTransform.DOKill();

            downButtonWindow.gameObject.SetActive(true);
            background.gameObject.SetActive(true);

            downButtonWindow.rectTransform.DOAnchorPosY(150, UI_MOVE_DURATION);

            background.color = background.color.GetFade(0);
            background.DOFade(0.3f, UI_MOVE_DURATION);
        }

        public void DeActive()
        {
            if (lastDownIndex >= 0)
                downButtonPopup[lastDownIndex].DeActive();

            background.DOKill();
            downButtonWindow.DOKill();

            downButtonWindow.rectTransform.DOAnchorPosY(-10, UI_MOVE_DURATION);

            background.color = background.color.GetFade(0.3f);
            background.DOFade(0f, UI_MOVE_DURATION).OnComplete(() => gameObject.SetActive(false));
        }
    }
}