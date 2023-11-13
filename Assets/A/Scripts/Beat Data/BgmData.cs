using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class BgmData
{
    public string bgmNickName;
    [Space(10f)] public string bgmName;

    [Title("High Light Color")]
    public List<ThemeColor> highLightColors = new();
    
    [Title("Special Value")]
    public float bpm;
    public float bpmMultiplier = 1;
    public float speedAdder;

    public float LastBeat
    {
        get
        {
            if (lastBeat <= 0)
                lastBeat = beatDataList.Last().beat;
            return lastBeat;
        }
    }

    private float lastBeat = -1;

    [Space(10f)] [Title("Beat Data")] [SerializeField]
    private TextAsset textAsset;
    
    [Space(5f)] [ShowIf("@textAsset != null")][TableList]
    public Queue<BeatData> beatDataList = new();

    [Button("Convert Csv To Beat Datas")]
    [ShowIf("@textAsset != null")]
    private void Convert()
    {
        beatDataList = new Queue<BeatData>();
        string[] rows = textAsset.text.Split('\n');

        BeatData prevData = null;
        foreach (var row in rows)
        {
            if (string.IsNullOrWhiteSpace(row) || string.IsNullOrEmpty(row)) continue;
            
            string[] columns = row.Split(",");
            float beat = float.Parse(columns[0]);
            var beatData = new BeatData
            {
                beat = beat,
                type = columns.Length <= 1 ? BeatType.Default : Utility.GetEnum<BeatType>(columns[1]),
                value = columns.Length <= 2 ? -1 : float.Parse(columns[2])
            };
            if (prevData != null)
                prevData.beatDistance = beat - prevData.beat;
            prevData = beatData;

            beatDataList.Enqueue(beatData);
        }

        Debug.Log("Add All Beat Datas");
    }
}

[Serializable]
public class TileChangeData
{
    public float pos;
    public float changeValue;
}

[Serializable]
public class BeatData
{
    public float beatDistance;
    public float beat;

    public BeatType type = BeatType.Default;
    public float value = -1;
}

public enum BeatType
{
    Default,
    Start,
    SpeedUp,
    SpeedDown,
    HighLight,
    End
}