﻿using UnityEngine;

namespace Lobby
{
    public class LobbyManager : Singleton<LobbyManager>, ISceneLink
    {
        public LobbyUIManager uiManager;

        public void Active()
        {
            GameManager.Instance.isGaming = false;
            
            Player.Instance.transform.position = Vector3.zero;
            Player.Instance.Reset();
            
            TileManager.Instance.StageReset();
            TileManager.Instance.Reset(0);
            
            uiManager.gameObject.SetActive(true);
            uiManager.ActiveSetting();
        }

        public void DeActive()
        {
            uiManager.DeActiveSetting();
        }
    }
}