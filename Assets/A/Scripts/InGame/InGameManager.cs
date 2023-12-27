using System.Collections.Generic;
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
        private List<int> beatScores = new List<int>();
        public void AddBeatHit(BeatHitType type)
        {
            int index = (int)type;
            if (beatScores.Count <= index)
                for (int i = beatScores.Count; i <= index; i++)
                    beatScores.Add(0);

            beatScores[index]++;
        }

        public int GetBeatHit(BeatHitType type)
        {
            int index = (int)type;
            if (beatScores.Count <= index)
                for (int i = beatScores.Count; i <= index; i++)
                    beatScores.Add(0);

            return beatScores[index];
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
            beatScores.Clear();

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

            this.Invoke(() =>
            {
                GameManager.Instance.ActiveSceneLink(SceneLinkType.Lobby);
            }, duration);
        }
    }
}