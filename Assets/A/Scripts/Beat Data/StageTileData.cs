using System;
using System.Collections.Generic;
using InGame;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[CreateAssetMenu(fileName = "TileData", menuName = "A/TileData", order = 0)]
public class StageTileData : SerializedScriptableObject
{
    [Title("Theme Color")] 
    public ThemeColor defaultColor;

    [Title("Enemy")]
    public List<Enemy> flyingEnemies;
    public List<Enemy> defaultEnemies;
    
    [Title("Bgm Data")]
    [NonSerialized, OdinSerialize]
    public List<BgmData> bgmDataList = new();

    [Title("Tiles")] 
    public List<RoadTileData> outGameTileDataList = new();
    public List<RoadTileData> roadTileDataList = new();
    public List<TileDataList> tileDataList = new();
}

[Serializable]
public struct ThemeColor
{
    public Color mainColor;
    public Color fogColor;
}

[Serializable]
public class TileDataList
{
    public List<TileData> dataList;
}
