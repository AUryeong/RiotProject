using System.Collections.Generic;
using UnityEngine;

public class RoadTileData : MonoBehaviour
{
    public List<RoadData> roadDatas;
    public float length;
}

[System.Serializable]
public class RoadData
{
    public float length;
    public List<int> lineCondition = new List<int>();
}