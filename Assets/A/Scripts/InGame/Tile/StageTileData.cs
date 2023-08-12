using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileData", menuName = "TileData", order = 0)]
public class StageTileData : ScriptableObject
{
    public List<RoadTileData> roadTileDataList = new();
    public List<TileDataList> tileDataList = new();
}

[System.Serializable]
public class TileDataList
{
    public List<TileData> dataList;
}
