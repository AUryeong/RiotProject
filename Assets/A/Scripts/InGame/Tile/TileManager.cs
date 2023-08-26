using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileManager : Singleton<TileManager>
{
    public const float TILE_DISTANCE = 2.5f;

    private const float BEAT_RENDER_DISTANCE = 30;
    private const float ENEMY_SYNC_START_POS = -3f;
    private const float PLAYER_RENDER_DISTANCE = 70;

    [Header("Stage")] [SerializeField] private List<StageTileData> stageTileDataList;
    [HideInInspector] public StageTileData stageTileData;

    [Header("Tiles")] private float roadTileLength;
    private readonly List<RoadTileData> createdRoadTileDataList = new();
    private List<int> lastRoadTileCondition;

    private readonly List<float> tileLengthList = new();
    private readonly List<GameObject> createdTileList = new();

    private float beatSpawnDuration;

    [Header("Bgm")] [SerializeField] private float bpmDistanceMultiplier;
    [HideInInspector] public BgmData bgmData;
    private float beatInterval;

    private bool isAutoTilling;

    private Dictionary<string, Queue<BeatData>> bgmNameToBeatData = new(); // ScriptableObject를 건들면 원본도 변경되기에 만든 Queue

    protected override void OnCreated()
    {
        base.OnCreated();
        StageReset();
    }

    private void StageReset()
    {
        bgmNameToBeatData = new Dictionary<string, Queue<BeatData>>();

        stageTileData = stageTileDataList[0];
        SetBgmData(stageTileData.bgmDataList[0]);
    }

    private void Start()
    {
        CreateTile();
    }

    private void CreateTile()
    {
        roadTileLength = 0;
        for (int index = 0; index < stageTileData.tileDataList.Count; index++)
            tileLengthList.Add(0);

        var data = stageTileData.roadTileDataList[0];

        var obj = PoolManager.Instance.Init(data.name);
        obj.transform.position = new Vector3(0, 0, roadTileLength);
        createdTileList.Add(obj);

        createdRoadTileDataList.Add(data);
        lastRoadTileCondition = data.roadDatas[^1].lineCondition;
        roadTileLength += data.length;

        float playerPos = Player.Instance.transform.position.z;
        while (roadTileLength - playerPos < PLAYER_RENDER_DISTANCE)
            CheckRoadTile(playerPos);
    }

    private void Update()
    {
        CheckTile();
        CheckRemoveTile();
    }

    private void CheckRemoveTile()
    {
        float playerPos = Player.Instance.transform.position.z;

        foreach (var createdTile in createdTileList)
        {
            if (!createdTile.gameObject.activeSelf) continue;
            if (playerPos - createdTile.transform.position.z > PLAYER_RENDER_DISTANCE)
                createdTile.gameObject.SetActive(false);
        }
    }

    private void CheckTile()
    {
        if (!Player.Instance.IsAlive) return;
        
        float playerPos = Player.Instance.transform.position.z;
        CheckRoadTile(playerPos);
        CheckBeatTile(playerPos);
        CheckBackgroundTile(playerPos);
    }

    private void CheckRoadTile(float playerPos)
    {
        if (roadTileLength - playerPos >= PLAYER_RENDER_DISTANCE) return;

        var data = stageTileData.roadTileDataList.FindAll(tileData => tileData.roadDatas[0].lineCondition.Intersect(lastRoadTileCondition).Any()).SelectOne();

        var roadTileObj = PoolManager.Instance.Init(data.name);
        roadTileObj.transform.position = new Vector3(0, 0, roadTileLength);

        if (!createdTileList.Contains(roadTileObj))
            createdTileList.Add(roadTileObj);

        if (createdTileList.Contains(roadTileObj))
            createdRoadTileDataList.Remove(data);

        createdRoadTileDataList.Add(data);
        lastRoadTileCondition = data.roadDatas[^1].lineCondition;
        roadTileLength += data.length;
    }

    private RoadData GetLengthToRoadData(float length)
    {
        float roadLength = roadTileLength;
        int lastIndex = createdRoadTileDataList.Count;
        while (roadLength > length)
        {
            lastIndex--;
            if (lastIndex <= 0)
                break;
            roadLength -= createdRoadTileDataList[lastIndex].length;
        }

        RoadData lastRoadData = null;
        foreach (var data in createdRoadTileDataList[lastIndex].roadDatas)
        {
            roadLength += data.length;
            lastRoadData = data;
            if (roadLength > length)
                break;
        }

        return lastRoadData;
    }

    private void CheckBeatTile(float playerPos)
    {
        beatSpawnDuration -= Time.deltaTime;
        if (beatSpawnDuration > 0) return;

        float length = playerPos + BEAT_RENDER_DISTANCE * (Player.Instance.Speed / Player.Instance.originSpeed);
        var lastRoadData = GetLengthToRoadData(length);

        if (!isAutoTilling)
        {
            var beatData = bgmNameToBeatData[bgmData.bgmName].Dequeue();
            if (Math.Abs(bgmData.startBeat - beatData.beat) < 0.01f)
                InGameManager.Instance.ChangeStage(length + ENEMY_SYNC_START_POS);

            switch (beatData.type)
            {
                case  BeatType.Default:
                    var enemyNodeObj = PoolManager.Instance.Init(nameof(Enemy));
                    enemyNodeObj.transform.position = new Vector3(lastRoadData.lineCondition.SelectOne() * TILE_DISTANCE, 0, length);
                    if (!createdTileList.Contains(enemyNodeObj))
                        createdTileList.Add(enemyNodeObj);
                    break;
                case BeatType.SpeedUp:
                    InGameManager.Instance.ChangeSpeedByBeatData(length, beatData.value);
                    break;
                case BeatType.SpeedDown:
                    InGameManager.Instance.ChangeSpeedByBeatData(length, -beatData.value);
                    break;
                case BeatType.HighLightOn:
                    InGameManager.Instance.ChangeHighLight(length, true);
                    break;
                case BeatType.HighLightOff:
                    InGameManager.Instance.ChangeHighLight(length, false);
                    break;
            }

            beatSpawnDuration += beatInterval * beatData.beatDistance;
        }

        // int random = Random.Range(0, 6);
        //
        // var enemyObj = PoolManager.Instance.Init(nameof(Enemy));
        // enemyObj.transform.position = new Vector3(lastRoadData.lineCondition.SelectOne() * TILE_DISTANCE, 0, length);
        //
        // if (!createdTileList.Contains(enemyObj))
        //     createdTileList.Add(enemyObj);
        //
        // if (random <= 2)
        // {
        //     var enemySubObj = PoolManager.Instance.Init(nameof(Enemy));
        //     enemySubObj.transform.position = new Vector3(lastRoadData.lineCondition.SelectOne() * TILE_DISTANCE, 0, length + beatLength / 2);
        //
        //     if (!createdTileList.Contains(enemySubObj))
        //         createdTileList.Add(enemySubObj);
        //     enemyLength += beatLength;
        // }
        //
        // if (random <= 0)
        // {
        //     var enemySubObj = PoolManager.Instance.Init(nameof(Enemy));
        //     enemySubObj.transform.position = new Vector3(lastRoadData.lineCondition.SelectOne() * TILE_DISTANCE, 0, length + beatLength);
        //
        //     if (!createdTileList.Contains(enemySubObj))
        //         createdTileList.Add(enemySubObj);
        //     enemyLength += beatLength;
        // }
        //
        // enemyLength += beatLength * 2;
        // if (enemyLength >= stageChangeLength)
        // {
        //     var stageData = stageTileDataList.FindAll(data => data != stageTileData).SelectOne();
        //     stageTileData = stageData;
        //
        //     stageChangeLength += STAGE_CHANGE_DISTANCE;
        //
        //     SetBgmData(stageData.bgmDataList.SelectOne());
        // }
    }

    private void SetBgmData(BgmData setBgmData)
    {
        var prevBgmData = bgmData;
        bgmData = setBgmData;
        beatInterval = 60f / bgmData.bpm * bpmDistanceMultiplier;

        isAutoTilling = bgmData.beatDataList == null || bgmData.beatDataList.Count <= 0;

        Player.Instance.speedAddValue += -prevBgmData.speedAdder + bgmData.speedAdder;
        if (isAutoTilling) return;

        if (!bgmNameToBeatData.ContainsKey(bgmData.bgmName))
        {
            bgmNameToBeatData.Add(bgmData.bgmName, new Queue<BeatData>(bgmData.beatDataList));
        }
    }

    private void CheckBackgroundTile(float playerPos)
    {
        for (var index = 0; index < stageTileData.tileDataList.Count; index++)
        {
            if (tileLengthList[index] - playerPos >= PLAYER_RENDER_DISTANCE + ENEMY_SYNC_START_POS) continue;

            var stage = stageTileData.tileDataList[index];
            var data = stage.dataList.SelectOne();

            var obj = PoolManager.Instance.Init(data.name);
            obj.transform.position = new Vector3(0, 0, tileLengthList[index]);

            if (!createdTileList.Contains(obj))
                createdTileList.Add(obj);

            tileLengthList[index] += data.length;
        }
    }
}