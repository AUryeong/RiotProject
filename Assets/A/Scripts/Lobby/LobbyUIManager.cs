using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class LobbyUIManager : MonoBehaviour, IActiveLink
    {
        [SerializeField] private Image background;

        [Header("Buttons")]
        [SerializeField] private Button[] downButtons;
        [SerializeField] private LobbyUIActiveLink[] downButtonPopup;

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

            foreach (var lobbyActiveLink in downButtonPopup)
                lobbyActiveLink.gameObject.SetActive(false);
        }

        private void DownButtonClick(int index)
        {
            if (lastDownIndex >= 0)
            {
                downButtonPopup[lastDownIndex].DeActive();
                downButtons[lastDownIndex].image.DOColor(Color.white, 0.4f);
                if (downButtonPopup[lastDownIndex] == downButtonPopup[index])
                {
                    lastDownIndex = -1;
                    return;
                }
            }

            downButtons[index].image.DOColor(new Color(0.75f, 0.75f, 0.75f), 0.4f);

            downButtonPopup[index].Active();
            lastDownIndex = index;
        }

        public void Active()
        {
            DownButtonClick(2);

            background.DOKill();
            background.gameObject.SetActive(true);

            background.color = background.color.GetFade(0);
            background.DOFade(0.3f, 0.5f);
        }

        public void DeActive()
        {
            if (lastDownIndex >= 0)
                downButtonPopup[lastDownIndex].DeActive();

            background.DOKill();

            background.color = background.color.GetFade(0.3f);
            background.DOFade(0f, 0.5f).OnComplete(() => background.gameObject.SetActive(false));
        }
    }
}