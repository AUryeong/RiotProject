using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class GameData
{
    public int rune;
    
    public int selectStageIndex;
    public int selectBgmIndex = 1;
}

public class SaveManager : Singleton<SaveManager>
{
    public string prefsName = "RunesPharmacy";

    [SerializeField] private GameData gameData; // 게임 데이터 확인용

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
        LoadGameData();
    }

    public void ResetSaveFile()
    {
        gameData = new GameData();
        SaveGameData();
        LoadGameData();
    }

    private void LoadGameData()
    {
        var s = PlayerPrefs.GetString(prefsName, "null");
        gameData = s.Equals("null") || string.IsNullOrEmpty(s) ? new GameData() : JsonUtility.FromJson<GameData>(s);
    }


    private void SaveGameData()
    {
        PlayerPrefs.SetString(prefsName, JsonUtility.ToJson(gameData));
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