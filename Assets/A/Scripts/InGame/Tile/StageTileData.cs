using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[CreateAssetMenu(fileName = "TileData", menuName = "A/TileData", order = 0)]
public class StageTileData : SerializedScriptableObject
{
    [Title("Theme Color")]
    public Color mainColor;
    public Color fogColor;

    [Title("Special Value")]
    public float speedAdder;
    
    [Title("Bgm Data")]
    [NonSerialized, OdinSerialize]
    public List<BgmData> bgmDataList = new();
    
    [Title("Tiles")]
    public List<RoadTileData> roadTileDataList = new();
    public List<TileDataList> tileDataList = new();
}

[Serializable]
public class TileDataList
{
    public List<TileData> dataList;
}
