using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct StageData
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
    public int selectBgmIndex = 1;

    public StageData[] stageDataList = new StageData[9];

    public float beatSync = 0;
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
        gameData = s.Equals("null") || string.IsNullOrEmpty(s) ? new GameData() : JsonUtility.FromJson<GameData>(s);
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