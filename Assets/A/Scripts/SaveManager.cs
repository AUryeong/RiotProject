using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class GameData
{
    public int rune;
    
    public int selectStageIndex;
    public int selectBgmIndex = 1;
    public int selectEffectIndex = 0;

    public bool isButton;
    
    public List<int> lastScores;

    public int GetLastScore(int index)
    {
        if (lastScores.Count <= index)
            for (int i = lastScores.Count; i <= index; i++)
                lastScores.Add(0);

        return lastScores[index];
    }
    
    public List<int> highScores;

    public int GetHighScore(int index)
    {
        if (highScores.Count <= index)
            for (int i = highScores.Count; i <= index; i++)
                highScores.Add(0);

        return highScores[index];
    }

    public float beatSync = 0;
}

public class SaveManager : Singleton<SaveManager>
{
    public string prefsName = "RunesPharmacy";

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