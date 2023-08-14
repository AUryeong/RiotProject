using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class BgmData
{
    [Space(10f)] public string bgmName;
    public float bpm;

    [Space(10f)] [Title("Beat Data")] [SerializeField]
    private TextAsset textAsset;

    [Space(5f)] [ShowIf("@textAsset != null")] [ReadOnly] [TableList]
    public Queue<BeatData> beatDataList = new();

    [Button("Convert Csv To Beat Datas")]
    [ShowIf("@textAsset != null")]
    private void Convert()
    {
        beatDataList = new Queue<BeatData>();
        string[] rows = textAsset.text.Split('\n');

        float lastBeatDataBeat = 0;
        foreach (var row in rows)
        {
            string[] columns = row.Split(",");
            float beat = float.Parse(columns[0]);
            var beatData = new BeatData
            {
                beat = beat,
                beatDistance = beat - lastBeatDataBeat,
                type = columns.Length <= 1 ? BeatType.Default : Utility.GetEnum<BeatType>(columns[1])
            };
            lastBeatDataBeat = beat;

            beatDataList.Enqueue(beatData);
        }

        Debug.Log("Add All Beat Datas");
    }
}

[Serializable]
public class BeatData
{
    public float beatDistance;
    public float beat;
    public BeatType type = BeatType.Default;
}

public enum BeatType
{
    Default,
    SpeedUp,
    SpeedDown
}