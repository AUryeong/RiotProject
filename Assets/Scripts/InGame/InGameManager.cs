using System.Collections.Generic;
using System.Linq;

namespace InGame
{
    public class InGameManager : Singleton<InGameManager>, IActiveLink
    {
        public InGameUIManager uiManager;

        public int Rune
        {
            get => rune;
            set
            {
                rune = value;
                uiManager.UpdateRune(rune);
            }
        }

        private int rune;
        private readonly List<int> beatScores = new();

        public void AddBeatHit(BeatHitType type)
        {
            int index = (int)type;
            if (beatScores.Count <= index)
                for (int i = beatScores.Count; i <= index; i++)
                    beatScores.Add(0);

            beatScores[index]++;
        }

        public int BeatHitCount => beatScores.Sum();

        public int GetBeatHit(BeatHitType type)
        {
            if (type == BeatHitType.Miss)
                return TileManager.Instance.bgmData.DefaultBeatCount - BeatHitCount;

            int index = (int)type;
            if (beatScores.Count <= index)
                for (int i = beatScores.Count; i <= index; i++)
                    beatScores.Add(0);

            return beatScores[index];
        }

        public void Active()
        {
            beatScores.Clear();

            GameManager.Instance.isGaming = true;
            SoundManager.Instance.PlaySound("", ESoundType.Bgm);

            TileManager.Instance.Reset();
            TileManager.Instance.CreateTile();

            Player.Instance.GameStart();

            uiManager.Active();
        }

        public void DeActive()
        {
            Rune = 0;
            beatScores.Clear();

            uiManager.gameObject.SetActive(false);
        }

        public void GameOver()
        {
            SoundManager.Instance.PlaySound("", ESoundType.Bgm);

            SaveManager.Instance.GameData.rune += Rune * (SaveManager.Instance.GameData.selectStageIndex + 1);

            var stageData = SaveManager.Instance.GameData.GetSelectStageData();
            stageData.lastScore = Rune;

            if (stageData.highScore < Rune)
                stageData.highScore = Rune;

            uiManager.DeActive();
        }
    }
}