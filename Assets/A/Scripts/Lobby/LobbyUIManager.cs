using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

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

        private LobbyUIActiveLink beforeActiveLink;

        public void Select(LobbyType type)
        {
            beforeActiveLink?.DeActive();
            switch (type)
            {
                case LobbyType.Home:
                    beforeActiveLink = uiPlay;
                    break;
                case LobbyType.Stage:
                    beforeActiveLink = uiStage;
                    break;
            }
            beforeActiveLink?.Active();
            
        }
        public void Active()
        {
            Select(LobbyType.Home);
        }


        public void DeActive()
        {
            beforeActiveLink?.DeActive();
            beforeActiveLink = null;
        }
    }
}