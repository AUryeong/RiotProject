using UnityEngine;

namespace Lobby
{
    public class LobbyManager : Singleton<LobbyManager>, ISceneLink
    {
        public LobbyUIManager uiManager;

        public void Active()
        {
            uiManager.gameObject.SetActive(true);
            uiManager.ActiveSetting();

            GameManager.Instance.isGaming = false;
            
            Player.Instance.Reset();
            Player.Instance.transform.position = Vector3.zero;
            
            TileManager.Instance.Reset(0);
        }

        public void DeActive()
        {
            uiManager.DeActiveSetting();
        }
    }
}