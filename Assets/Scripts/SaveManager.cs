using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StageData
{
    public bool isBuy;
    public int lastScore;
    public int highScore;
}

[Serializable]
public class GameData
{
    public int rune;

    public int selectStageIndex;
    public int selectBgmIndex;

    public StageData GetStageData(int stageIndex, int bgmIndex)
    {
        return stageDataList[stageIndex * 3 + bgmIndex];
    }

    public StageData GetSelectStageData()
    {
        return GetStageData(selectStageIndex, selectBgmIndex);
    }

    public List<StageData> stageDataList = new(9);

    public float beatSync;
    public float bgmSoundMultiplier = 1;
    public float sfxSoundMultiplier = 1;
}

public class SaveManager : Singleton<SaveManager>
{
    private const string SAVE_DATA_NAME = "RatsGo";

    private GameData gameData;

    public GameData GameData
    {
        get
        {
            if (gameData == null)
                LoadGameData();
            return gameData;
        }
    }

    protected override void OnCreated()
    {
        if (gameData == null)
            LoadGameData();
    }

    [Button(nameof(ResetSaveFile))]
    private void ResetSaveFile()
    {
        gameData = null;
        SaveGameData();
    }

    private void LoadGameData()
    {
        var s = PlayerPrefs.GetString(SAVE_DATA_NAME, "null");
        if (string.IsNullOrEmpty(s) || s.Equals("null"))
        {
            gameData = new GameData();
            var stageDataList = new List<StageData>();
            for (int i = 0; i < 9; i++)
            {
                var stageData = new StageData();
                stageDataList.Add(stageData);
                if (i % 3 == 0)
                    stageData.isBuy = true;
            }

            gameData.stageDataList = stageDataList;
        }
        else
        {
            gameData = JsonUtility.FromJson<GameData>(s);
        }
    }


    private void SaveGameData()
    {
        PlayerPrefs.SetString(SAVE_DATA_NAME, JsonUtility.ToJson(gameData));
    }

    private void OnApplicationQuit()
    {
        SaveGameData();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
            SaveGameData();
    }
}