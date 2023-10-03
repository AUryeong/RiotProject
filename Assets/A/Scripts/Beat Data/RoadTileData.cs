using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class RoadTileData : MonoBehaviour
{
    public List<RoadData> roadDatas;
    public float length;
}

[System.Serializable]
public class RoadData
{
    public float length;
    
    [HideInInspector] public int summonLine;
    
    public List<int> lineCondition = new();
    public bool isJustBlank;
}