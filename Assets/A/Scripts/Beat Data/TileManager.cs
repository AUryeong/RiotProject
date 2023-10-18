﻿using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using InGame;
using UnityEngine;

public class TileManager : Singleton<TileManager>
{
    public const float TILE_DISTANCE = 2.5f;

    private const float BEAT_RENDER_DISTANCE = 20;
    private const float BEAT_SYNC_START_POS = -3f;

    private const float PLAYER_RENDER_DISTANCE = 70;
    private const float PLAYER_REMOVE_DISTANCE = 60;

    [SerializeField] private GlobalObjectFogController fogController;

    [Header("Stage")] public List<StageTileData> stageTileDataList;
    [HideInInspector] public StageTileData stageTileData;

    [Header("Tiles")] private float stageStartPos;
    private float roadStartPos;
    private float roadTileLength;
    private readonly List<RoadTileData> createdRoadTileDataList = new();
    private readonly List<GameObject> createdBackgroundList = new();
    private int lastSummonLine;

    private readonly List<float> tileLengthList = new();
    private readonly List<GameObject> createdTileList = new();

    private float beatSpawnDuration;

    [Header("Bgm")][SerializeField] private float bpmDistanceMultiplier;
    [HideInInspector] public BgmData bgmData;
    private List<Enemy> enemies = new();
    public float beatInterval;
    private int beat;
    private float beatSyncPos;

    private bool isEndBgm;
    private Queue<BeatData> beatDataQueue = new(); // ScriptableObject를 건들면 원본도 변경되기에 만든 Queue

    [Header("Change")] private float beatStartPos;

    private readonly List<TileChangeData> speedDataList = new();
    private readonly List<TileChangeData> highLightDataList = new();
    public int highLightLevel;

    private bool isBeatCreating;

    private bool isChangeStage;
    private float stageChangePos;

    private bool isChangeEndBgm;
    private float endBgmPos;

    [Header("Item")][SerializeField] private List<string> itemList;

    private int itemMaxValue;

    private const float ITEM_RANDOM_PROB_VALUE_PERCENT = 1f;
    private const float ITEM_RANDOM_PROB_MAX = 50;

    protected override void OnCreated()
    {
        base.OnCreated();
        foreach (var stageTileData in stageTileDataList)
            stageTileData.Init();

        StageReset();
        Reset(0);
    }

    public void StageReset()
    {
        stageTileData = stageTileDataList[SaveManager.Instance.GameData.selectStageIndex];
        SaveManager.Instance.GameData.selectBgmIndex %= stageTileData.bgmDataList.Count;
        SetBgmData(stageTileData.bgmDataList[SaveManager.Instance.GameData.selectBgmIndex]);
    }

    public void Reset(float startPos = -1)
    {
        if (startPos >= 0)
        {
            roadTileLength = startPos;

            tileLengthList.Clear();
            for (int index = 0; index < stageTileData.tileDataList.Count; index++)
                tileLengthList.Add(startPos);

            foreach (var createdTile in createdTileList)
                createdTile.gameObject.SetActive(false);
            createdTileList.Clear();

            enemies.Clear();
        }

        isChangeStage = false;
        isBeatCreating = false;
        isEndBgm = false;

        ChangeThemeColor(stageTileData.defaultColor);

        highLightLevel = 0;
        highLightDataList.Clear();

        speedDataList.Clear();

        stageStartPos = roadTileLength;
        beatStartPos = stageStartPos - 6;
        roadStartPos = stageStartPos;

        beatSyncPos = SaveManager.Instance.GameData.beatSync;

        beatSpawnDuration = 0;
        beat = 0;

        createdRoadTileDataList.Clear();
    }

    public void CreateTile()
    {
        roadTileLength = stageStartPos;

        lastSummonLine = 0;

        CreateRoadTile();
        CreateRoadTile();
        CreateRoadTile();
        CreateRoadTile();
        CreateRoadTile();

        //while (roadTileLength - playerPos < PLAYER_RENDER_DISTANCE)
        //    CheckCreateRoadTile(playerPos);

        //while (itemTileLength - playerPos < PLAYER_RENDER_DISTANCE)
        //    CheckCreateItemTile(playerPos);
    }

    private void Update()
    {
        if (GameManager.Instance.isGaming && !isEndBgm)
            GamingUpdate();
        else
            OutGamingUpdate();
    }

    private void FixedUpdate()
    {
        CheckRemoveTile();
    }

    private void OutGamingUpdate()
    {
        float playerPos = Player.Instance.transform.position.z;
        CheckOutGameRoadTile(playerPos);
        CheckCreateBackgroundTile(playerPos);
    }

    private void GamingUpdate()
    {
        CheckCreateTile();
        CheckChange();
    }

    #region TileCheck

    private void CheckOutGameRoadTile(float playerPos)
    {
        if (roadTileLength - playerPos >= PLAYER_RENDER_DISTANCE) return;

        var data = stageTileData.outGameTileDataList.SelectOne();

        var roadTileObj = PoolManager.Instance.Init(data.name);
        roadTileObj.transform.position = new Vector3(0, 0, roadTileLength);

        if (!createdTileList.Contains(roadTileObj))
            createdTileList.Add(roadTileObj);

        roadTileLength += data.length;
    }

    private void CheckCreateTile()
    {
        if (!Player.Instance.IsAlive) return;

        float playerPos = Player.Instance.transform.position.z;
        CheckCreateBeatTile(playerPos);
        CheckCreateBackgroundTile(playerPos);
    }

    private void CreateItemTile(float itemPos, RoadTileData roadTileData)
    {
        GameObject itemObj;
        if (itemMaxValue >= ITEM_RANDOM_PROB_MAX && Random.Range(0f, 100f) <= ITEM_RANDOM_PROB_VALUE_PERCENT)
        {
            itemObj = PoolManager.Instance.Init(itemList.SelectOne());
            itemMaxValue = 0;
        }
        else
        {
            itemObj = PoolManager.Instance.Init(nameof(Item_Rune));
            itemMaxValue++;
        }

        itemObj.transform.position = new Vector3(roadTileData.summonLine * TILE_DISTANCE, 1, itemPos);

        if (!createdTileList.Contains(itemObj))
            createdTileList.Add(itemObj);
    }

    private void CreateRoadTile(bool isbeatTiming = false)
    {
        RoadTileData data;
        if (isbeatTiming)
            data = stageTileData.roadTileDataList.FindAll(tileData => !tileData.lineCondition.Contains(lastSummonLine)).SelectOne();
        else
            data = stageTileData.roadTileDataList.FindAll(tileData => tileData.lineCondition.Contains(lastSummonLine)).SelectOne();

        var roadTileObj = PoolManager.Instance.Init(data.name);
        roadTileObj.transform.position = new Vector3(0, 0, roadTileLength);

        if (!createdTileList.Contains(roadTileObj))
            createdTileList.Add(roadTileObj);

        data = roadTileObj.GetComponent<RoadTileData>();

        createdRoadTileDataList.Add(data);
        var lineConditions = data.lineCondition.FindAll(condition => Mathf.Abs(condition - lastSummonLine) <= 1);

        if (!lineConditions.Contains(lastSummonLine) || Random.Range(0, 2) != 0)
            lastSummonLine = lineConditions.Count > 0 ? lineConditions.SelectOne() : data.lineCondition.SelectOne();
        data.summonLine = lastSummonLine;

        roadTileLength += data.length;
        CreateItemTile(roadTileLength - data.length / 2, data);

        if (roadTileLength - roadStartPos < PLAYER_RENDER_DISTANCE * 1.5f) return;

        roadStartPos += createdRoadTileDataList[0].length;
        createdRoadTileDataList.RemoveAt(0);
    }

    private void CheckCreateBeatTile(float playerPos)
    {
        if (isEndBgm) return;

        if (!isBeatCreating)
        {
            if (Player.Instance.transform.position.z < beatStartPos) return;

            isBeatCreating = true;
            return;
        }
        bool isBeatTiming = false;

        beatSpawnDuration += Time.deltaTime / beatInterval;
        if (beatDataQueue.Peek().beat <= (beatSpawnDuration + beat) * bgmData.bpmMultiplier)
        {
            CreateBeatData(playerPos);
            isBeatTiming = true;
        }

        if (beatSpawnDuration < 1) return;

        foreach (var obj in createdBackgroundList)
        {
            obj.transform.DOKill();
            obj.transform.DOMoveY(-1, beatInterval / 4).SetLoops(2, LoopType.Yoyo);
        }

        beatSpawnDuration--;
        beat++;

        CreateRoadTile(isBeatTiming);
    }

    private void CreateBeatData(float playerPos)
    {
        float length = playerPos + BEAT_RENDER_DISTANCE * (Player.Instance.Speed / Player.Instance.originSpeed);

        var beatData = beatDataQueue.Dequeue();

        switch (beatData.type)
        {
            case BeatType.Default:
                var lastRoadData = GetLengthToRoadData(length);
                var nextRoadData = GetLengthToRoadData(length + 3);
                var enemy = !nextRoadData.lineCondition.Contains(lastRoadData.summonLine) ? stageTileData.flyingEnemies.SelectOne() : stageTileData.defaultEnemies.SelectOne();

                var enemyNodeObj = PoolManager.Instance.Init(enemy.name);
                enemyNodeObj.transform.position = new Vector3(lastRoadData.summonLine * TILE_DISTANCE, 0, length) + enemy.transform.localPosition;

                if (!createdTileList.Contains(enemyNodeObj))
                {
                    enemies.Add(enemyNodeObj.GetComponent<Enemy>());
                    createdTileList.Add(enemyNodeObj);
                }

                break;
            case BeatType.Start:
                ChangeStage(length + BEAT_SYNC_START_POS + beatSyncPos);
                break;
            case BeatType.SpeedUp:
                //    ChangeSpeedByBeatData(length, beatData.value);
                break;
            case BeatType.End:
                isEndBgm = true;
                ChangeThemeColor(stageTileData.defaultColor);
                InGameManager.Instance.ReturnLobby(10);
                Player.Instance.Boost(10);
                break;
            case BeatType.SpeedDown:
                //    ChangeSpeedByBeatData(length, -beatData.value);
                break;
            case BeatType.HighLight:
                ChangeHighLight(length, beatData.value);
                break;
        }
    }

    private void CheckCreateBackgroundTile(float playerPos)
    {
        for (var index = 0; index < stageTileData.tileDataList.Count; index++)
        {
            if (tileLengthList[index] - playerPos >= PLAYER_RENDER_DISTANCE + BEAT_SYNC_START_POS + beatSyncPos) continue;

            var stage = stageTileData.tileDataList[index];
            var data = stage.dataList.SelectOne();

            var obj = PoolManager.Instance.Init(data.name);
            obj.transform.position = new Vector3(0, 0, tileLengthList[index]);

            if (index == 0)
                createdBackgroundList.Add(obj);

            if (!createdTileList.Contains(obj))
                createdTileList.Add(obj);

            tileLengthList[index] += data.length;
        }
    }

    #endregion

    public List<Enemy> GetEnemies()
    {
        return enemies.FindAll(enemy => enemy.gameObject.activeSelf);
    }

    #region Change

    private void CheckChange()
    {
        CheckChangeHighLight();
        CheckChangeSpeed();
        CheckChangeStage();
    }

    private void ChangeHighLight(float changeLength, float changeValue)
    {
        highLightDataList.Add(new TileChangeData()
        {
            pos = changeLength,
            changeValue = changeValue
        });
    }

    private void ChangeSpeedByBeatData(float changeLength, float changeValue)
    {
        speedDataList.Add(new TileChangeData()
        {
            pos = changeLength,
            changeValue = changeValue
        });
    }

    private void ChangeStage(float changeLength)
    {
        isChangeStage = true;
        stageChangePos = changeLength;
    }

    private void ChangeThemeColor(ThemeColor themeColor)
    {
        fogController.mainColor = themeColor.mainColor;
        fogController.fogColor = themeColor.fogColor;

        Player.Instance.outLine.OutlineColor = themeColor.fogColor.GetFade(0.3f);

        var transEffect = PoolManager.Instance.Init("Trans Effect");
        transEffect.transform.position = Player.Instance.transform.position;
    }

    private void CheckChangeHighLight()
    {
        if (highLightDataList.Count <= 0) return;

        bool isRemoveData = false;
        foreach (var changeData in highLightDataList)
        {
            if (Player.Instance.transform.position.z < changeData.pos) return;

            isRemoveData = true;

            highLightLevel = Mathf.Clamp(Mathf.RoundToInt(changeData.changeValue), 0, bgmData.highLightColors.Count);

            var changeStage = stageTileData;
            var themeColor = highLightLevel <= 0 ? changeStage.defaultColor : bgmData.highLightColors[highLightLevel - 1];

            ChangeThemeColor(themeColor);
        }

        if (isRemoveData)
            highLightDataList.RemoveAt(0);
    }

    private void CheckChangeStage()
    {
        if (!isChangeStage || Player.Instance.transform.position.z < stageChangePos) return;

        isChangeStage = false;
        isBeatCreating = true;

        var changeStage = stageTileData;
        var themeColor = changeStage.defaultColor;

        ChangeThemeColor(themeColor);

        SoundManager.Instance.PlaySound(bgmData.bgmName, ESoundType.Bgm, 0.5f);
    }

    private void CheckChangeSpeed()
    {
        if (speedDataList.Count <= 0) return;

        bool isRemoveData = false;
        foreach (var changeData in speedDataList)
        {
            if (Player.Instance.transform.position.z < changeData.pos) return;

            isRemoveData = true;

            Player.Instance.SpeedAddValue += changeData.changeValue;
        }

        if (isRemoveData)
            speedDataList.RemoveAt(0);
    }

    #endregion

    private void CheckRemoveTile()
    {
        float playerPos = Player.Instance.transform.position.z;

        foreach (var createdTile in createdTileList)
        {
            if (!createdTile.gameObject.activeSelf) continue;
            if (playerPos - createdTile.transform.position.z > PLAYER_REMOVE_DISTANCE)
            {
                if (createdBackgroundList.Contains(createdTile))
                    createdBackgroundList.Remove(createdTile);
                createdTile.gameObject.SetActive(false);
            }
        }
    }


    public void SetBgmData(BgmData setBgmData)
    {
        bgmData = setBgmData;
        beatInterval = 60f / bgmData.bpm * bpmDistanceMultiplier;

        Player.Instance.SpeedAddValue = bgmData.speedAdder;

        beatDataQueue = new Queue<BeatData>(bgmData.beatDataList);

        SoundManager.Instance.PlaySound(bgmData.bgmName, ESoundType.Bgm, 0.5f);

        if (bgmData.beatDataList.Count <= 0) return;

        BeatData beat = bgmData.beatDataList.ToList().Find(beatData => beatData.type == BeatType.HighLight && beatData.value > 0);

        if (beat == null) return;

        SoundManager.Instance.GetAudioSource(ESoundType.Bgm).time = beatInterval * beat.beat;
    }

    public RoadTileData GetLengthToRoadData(float length)
    {
        float roadLength = roadTileLength;
        int lastIndex = createdRoadTileDataList.Count;
        while (roadLength > length)
        {
            lastIndex--;
            roadLength -= createdRoadTileDataList[lastIndex].length;
        }

        return createdRoadTileDataList[lastIndex];
    }
}