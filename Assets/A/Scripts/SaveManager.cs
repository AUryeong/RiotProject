using Sirenix.OdinInspector;
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
    
    public List<int> lastScores = new List<int>();

    public int GetLastScore(int index)
    {
        if (lastScores.Count <= index)
            for (int i = lastScores.Count; i <= index; i++)
                lastScores.Add(0);

        return lastScores[index];
    }
    
    public List<int> highScores = new List<int>();

    public int GetHighScore(int index)
    {
        if (highScores.Count <= index)
            for (int i = highScores.Count; i <= index; i++)
                highScores.Add(0);

        return highScores[index];
    }

    public float beatSync = 0;
    public float bgmSoundMultiplier = 1;
    public float sfxSoundMultiplier = 1;
}

public class SaveManager : Singleton<SaveManager>
{
    public const string SAVE_DATA_NAME = "RatsGo";

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