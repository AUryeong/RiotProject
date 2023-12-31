﻿using System;
using System.Collections.Generic;
using InGame;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[CreateAssetMenu(fileName = "TileData", menuName = "A/TileData", order = 0)]
public class StageTileData : SerializedScriptableObject
{
    public string stageNickName;
    public Color uiColor;
    public Color uiDarkColor;

    [Title("Stage Color")] 
    public Color directionLightColor;
    public PostProcessProfile postProcessProfile;
    public Material skyBox;

    [Title("Theme Color")] 
    public ThemeColor defaultColor;

    [Title("Enemy")]
    public List<Enemy> flyingEnemies;
    public List<Enemy> defaultEnemies;
    
    [Title("Bgm Data")]
    [NonSerialized, OdinSerialize]
    public List<BgmData> bgmDataList = new();

    [Title("Tiles")] 
    public List<RoadTileData> roadTileDataList = new();
    public List<TileDataList> tileDataList = new();

    public void Init()
    {
        foreach (var tileData in roadTileDataList)
            PoolManager.Instance.JoinPoolingData(tileData.name, tileData.gameObject);

        foreach (var tileDatas in tileDataList)
            foreach (var tileData in tileDatas.dataList)
                PoolManager.Instance.JoinPoolingData(tileData.name, tileData.gameObject);

        foreach (var enemy in flyingEnemies)
            PoolManager.Instance.JoinPoolingData(enemy.name, enemy.gameObject);

        foreach (var enemy in defaultEnemies)
            PoolManager.Instance.JoinPoolingData(enemy.name, enemy.gameObject);
    }
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
