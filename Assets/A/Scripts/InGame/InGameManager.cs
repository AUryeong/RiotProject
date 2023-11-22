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
            int index = SaveManager.Instance.GameData.selectStageIndex * 3 + SaveManager.Instance.GameData.selectBgmIndex;
            if (SaveManager.Instance.GameData.lastScores.Count <= index)
                for (int i = SaveManager.Instance.GameData.lastScores.Count; i <= index; i++)
                    SaveManager.Instance.GameData.lastScores.Add(0);

            SaveManager.Instance.GameData.lastScores[index] = Rune;

            if (SaveManager.Instance.GameData.highScores.Count <= index)
                for (int i = SaveManager.Instance.GameData.highScores.Count; i <= index; i++)
                    SaveManager.Instance.GameData.highScores.Add(0);

            if (SaveManager.Instance.GameData.highScores[index] < Rune)
                SaveManager.Instance.GameData.highScores[index] = Rune;

            this.Invoke(() => GameManager.Instance.ActiveSceneLink(SceneLinkType.Lobby), duration);
        }
    }
}