using UnityEngine;

namespace InGame
{
    public class InGameManager : Singleton<InGameManager>, IActiveLink
    {
        private Camera mainCamera;
        private Vector3 cameraDistance;
        public InGameUIManager uiManager;

        public int Rune
        {
            get { return rune; }
            set
            {
                rune = value;
                uiManager.UpdateRune(rune);
            }
        }

        private int rune;

        private void Start()
        {
            mainCamera = Camera.main;
            cameraDistance = mainCamera.transform.position - Player.Instance.transform.position;
        }

        public void Active()
        {
            GameManager.Instance.isGaming = true;
            SoundManager.Instance.PlaySound("", ESoundType.Bgm);

            TileManager.Instance.Reset();
            TileManager.Instance.CreateTile();

            Player.Instance.GameStart();

            uiManager.gameObject.SetActive(true);
            uiManager.ActiveSetting();
        }

        public void DeActive()
        {
            Rune = 0;
            
            uiManager.gameObject.SetActive(false);
        }

        public void ReturnLobby(float duration = 3)
        {
            SoundManager.Instance.PlaySound("", ESoundType.Bgm);
            
            SaveManager.Instance.GameData.rune += Rune;

            this.Invoke(() => GameManager.Instance.ActiveSceneLink(SceneLinkType.Lobby), duration);
        }
    }
}