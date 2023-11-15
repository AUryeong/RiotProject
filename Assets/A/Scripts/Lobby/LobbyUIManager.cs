using UnityEngine;

namespace Lobby
{
    public enum LobbyType
    {
        Home,
        Stage,
        Shop
    }

    public class LobbyUIManager : MonoBehaviour, IActiveLink
    {
        [SerializeField] private LobbyUIPlay uiPlay;
        [SerializeField] private LobbyUIStage uiStage;

        private LobbyUIActiveLink activeLink;

        public void Select(LobbyType type)
        {
            if (activeLink != null)
                activeLink.DeActive();
            
            switch (type)
            {
                case LobbyType.Home:
                    activeLink = uiPlay;
                    break;
                case LobbyType.Stage:
                    activeLink = uiStage;
                    break;
                case LobbyType.Shop:
                    break;
            }

            activeLink.Active();
        }

        public void Active()
        {
            uiPlay.gameObject.SetActive(false);
            uiStage.gameObject.SetActive(false);

            Select(LobbyType.Home);
        }

        public void Bounce()
        {
            if (activeLink == uiPlay)
                uiPlay.Bounce();
        }
        
        public void DeActive()
        {
            activeLink.DeActive();
            activeLink = null;
        }
    }
}