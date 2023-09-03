using UnityEngine;

public class LobbyManager : Singleton<LobbyManager>
{
    public LobbyUIManager uiManager;

    private void Start()
    {
        SoundManager.Instance.PlaySound("Sound1_70BPM", ESoundType.Bgm, 0.5f);
    }
    public void GameStart()
    {
        SoundManager.Instance.PlaySound("", ESoundType.Bgm);
        GameManager.Instance.isGaming = true;

        TileManager.Instance.Reset();
        TileManager.Instance.CreateTile();
        
        Player.Instance.GameStart();
        
        InGameManager.Instance.uiManager.Active(true);
        uiManager.Active(false);
    }
}