﻿using UnityEngine;

namespace Lobby
{
    public class LobbyManager : Singleton<LobbyManager>, IActiveLink
    {
        public LobbyUIManager uiManager;

        public void Active()
        {
            GameManager.Instance.isGaming = false;
            
            Player.Instance.transform.position = Vector3.zero;
            Player.Instance.Reset();
            
            TileManager.Instance.StageReset();
            TileManager.Instance.TileReset();
            TileManager.Instance.Reset();
            
            uiManager.gameObject.SetActive(true);
            uiManager.Active();
        }

        public void DeActive()
        {
            uiManager.DeActive();
        }
    }
}