using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "TileData", menuName = "A/TileData", order = 0)]
public class StageTileData : SerializedScriptableObject
{
    [Title("Theme Color")] 
    public ThemeColor defaultColor;
    public ThemeColor highLightColor;

    [Title("Enemy")]
    public List<Enemy> flyingEnemies;
    public List<Enemy> defaultEnemies;
    
    [Title("Bgm Data")]
    [NonSerialized, OdinSerialize]
    public List<BgmData> bgmDataList = new();
    
    [Title("Tiles")]
    public List<RoadTileData> roadTileDataList = new();
    public List<TileDataList> tileDataList = new();
}

[Serializable]
public class ThemeColor
{
    public Color mainColor;
    public Color fogColor;
}

[Serializable]
public class TileDataList
{
    public List<TileData> dataList;
}
