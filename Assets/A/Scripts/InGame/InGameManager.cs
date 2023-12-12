using UnityEngine;

namespace InGame
{
    public class InGameManager : Singleton<InGameManager>, IActiveLink
    {
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

            SaveManager.Instance.GameData.rune += Rune * (SaveManager.Instance.GameData.selectStageIndex + 1);
            int index = SaveManager.Instance.GameData.selectStageIndex * 3 + SaveManager.Instance.GameData.selectBgmIndex;

            SaveManager.Instance.GameData.lastScores[index] = Rune;

            if (SaveManager.Instance.GameData.GetHighScore(index) < Rune)
                SaveManager.Instance.GameData.highScores[index] = Rune;

            this.Invoke(() => GameManager.Instance.ActiveSceneLink(SceneLinkType.Lobby), duration);
        }
    }
}