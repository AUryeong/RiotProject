using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class TileManager : Singleton<TileManager>
{
    public const float TILE_DISTANCE = 2.5f;

    private const float BEAT_RENDER_DISTANCE = 25;
    private const float BEAT_SYNC_START_POS = -3f;
    private const float PLAYER_RENDER_DISTANCE = 70;

    [SerializeField] private GlobalObjectFogController fogController;

    [Header("Stage")] [SerializeField] private List<StageTileData> stageTileDataList;
    [HideInInspector] public StageTileData stageTileData;

    [Header("Tiles")] private float roadStartPos = 0;
    private float roadTileLength;
    private readonly List<RoadTileData> createdRoadTileDataList = new();
    private List<int> lastRoadTileCondition;

    private readonly List<float> tileLengthList = new();
    private readonly List<GameObject> createdTileList = new();

    private float beatSpawnDuration;

    private float itemTileLength;

    [Header("Bgm")] [SerializeField] private float bpmDistanceMultiplier;
    [HideInInspector] public BgmData bgmData;
    private float beatInterval;

    private bool isAutoTilling;

    private Dictionary<string, Queue<BeatData>> bgmNameToBeatData = new(); // ScriptableObject를 건들면 원본도 변경되기에 만든 Queue


    [Header("Change")] private bool isChangeSpeed;
    private bool isChangeStage;
    private bool isChangeHighLight;

    private bool isHighLighted;

    private float changeSpeedValue;

    private float speedChangePos;
    private float stageChangePos;
    private float highLightChangePos;

    [Header("Item")] [SerializeField] private List<string> itemList;

    private int itemMaxValue = 0;
    private const float ITEM_RANDOM_PROB_VALUE_PERCENT = 1f;
    private const float ITEM_RANDOM_PROB_MAX = 50;

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
        itemTileLength = 3;
        for (int index = 0; index < stageTileData.tileDataList.Count; index++)
            tileLengthList.Add(0);

        var data = stageTileData.roadTileDataList[0];

        var obj = PoolManager.Instance.Init(data.name);
        obj.transform.position = new Vector3(0, 0, roadTileLength);
        createdTileList.Add(obj);

        if (createdRoadTileDataList.Contains(data))
            createdRoadTileDataList.Remove(data);

        createdRoadTileDataList.Add(data);

        lastRoadTileCondition = data.roadDatas[^1].lineCondition;
        roadTileLength += data.length;

        float playerPos = Player.Instance.transform.position.z;
        while (roadTileLength - playerPos < PLAYER_RENDER_DISTANCE)
            CheckRoadTile(playerPos);

        while (itemTileLength - playerPos < PLAYER_RENDER_DISTANCE)
            CheckItemTile(playerPos);
    }

    private void Update()
    {
        CheckTile();
        CheckRemoveTile();
        CheckChange();
    }

    #region TileCheck

    private void CheckTile()
    {
        if (!Player.Instance.IsAlive) return;

        float playerPos = Player.Instance.transform.position.z;
        CheckRoadTile(playerPos);
        CheckBeatTile(playerPos);
        CheckBackgroundTile(playerPos);
        CheckItemTile(playerPos);
    }

    private void CheckItemTile(float playerPos)
    {
        if (itemTileLength - playerPos >= PLAYER_RENDER_DISTANCE) return;

        var lastRoadData = GetLengthToRoadData(itemTileLength);

        if (!lastRoadData.isJustBlank)
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

            itemObj.transform.position = new Vector3(lastRoadData.lineCondition.SelectOne() * TILE_DISTANCE, 1, itemTileLength);

            if (!createdTileList.Contains(itemObj))
                createdTileList.Add(itemObj);
        }

        itemTileLength += lastRoadData.length;
    }

    private void CheckRoadTile(float playerPos)
    {
        if (roadTileLength - playerPos >= PLAYER_RENDER_DISTANCE) return;

        var data = stageTileData.roadTileDataList.FindAll(tileData => tileData.roadDatas[0].lineCondition.Intersect(lastRoadTileCondition).Any()).SelectOne();

        var roadTileObj = PoolManager.Instance.Init(data.name);
        roadTileObj.transform.position = new Vector3(0, 0, roadTileLength);

        if (!createdTileList.Contains(roadTileObj))
            createdTileList.Add(roadTileObj);

        createdRoadTileDataList.Add(data);
        lastRoadTileCondition = data.roadDatas[^1].lineCondition;
        roadTileLength += data.length;

        if (roadTileLength - roadStartPos >= PLAYER_RENDER_DISTANCE * 1.5f)
        {
            roadStartPos += createdRoadTileDataList[0].length;
            createdRoadTileDataList.RemoveAt(0);
        }
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

            switch (beatData.type)
            {
                case BeatType.Default:
                    var enemy = lastRoadData.isJustBlank ? stageTileData.flyingEnemies.SelectOne() : stageTileData.defaultEnemies.SelectOne();

                    var enemyNodeObj = PoolManager.Instance.Init(enemy.name);
                    enemyNodeObj.transform.position = new Vector3(lastRoadData.lineCondition.SelectOne() * TILE_DISTANCE, 0, length) + enemy.transform.localPosition;

                    if (!createdTileList.Contains(enemyNodeObj))
                        createdTileList.Add(enemyNodeObj);

                    break;
                case BeatType.Start:
                    ChangeStage(length + BEAT_SYNC_START_POS);
                    beatSpawnDuration += beatInterval * beatData.beatDistance;
                    return;
                case BeatType.SpeedUp:
                    ChangeSpeedByBeatData(length, beatData.value);
                    break;
                case BeatType.SpeedDown:
                    ChangeSpeedByBeatData(length, -beatData.value);
                    break;
                case BeatType.HighLightOn:
                    ChangeHighLight(length, true);
                    break;
                case BeatType.HighLightOff:
                    ChangeHighLight(length, false);
                    break;
            }

            beatSpawnDuration += beatInterval * beatData.beatDistance;
        }
    }

    private void CheckBackgroundTile(float playerPos)
    {
        for (var index = 0; index < stageTileData.tileDataList.Count; index++)
        {
            if (tileLengthList[index] - playerPos >= PLAYER_RENDER_DISTANCE + BEAT_SYNC_START_POS) continue;

            var stage = stageTileData.tileDataList[index];
            var data = stage.dataList.SelectOne();

            var obj = PoolManager.Instance.Init(data.name);
            obj.transform.position = new Vector3(0, 0, tileLengthList[index]);

            if (!createdTileList.Contains(obj))
                createdTileList.Add(obj);

            tileLengthList[index] += data.length;
        }
    }

    #endregion

    #region Change

    private void CheckChange()
    {
        CheckChangeHighLight();
        CheckChangeSpeed();
        CheckChangeStage();
    }

    private void ChangeHighLight(float changeLength, bool isHighLight)
    {
        isChangeHighLight = true;
        isHighLighted = isHighLight;
        highLightChangePos = changeLength;
    }

    private void ChangeSpeedByBeatData(float changeLength, float speed)
    {
        isChangeSpeed = true;
        changeSpeedValue = speed;
        speedChangePos = changeLength;
    }

    private void ChangeStage(float changeLength)
    {
        isChangeStage = true;
        stageChangePos = changeLength;
    }

    private void ChangeThemeColor(ThemeColor themeColor, bool isHighLightSkip = false)
    {
        if (!isHighLightSkip && isHighLighted) return;

        fogController.mainColor = themeColor.mainColor;
        fogController.fogColor = themeColor.fogColor;

        var transEffect = PoolManager.Instance.Init("Trans Effect");
        transEffect.transform.position = Player.Instance.transform.position;
    }

    private void CheckChangeHighLight()
    {
        if (!isChangeHighLight || Player.Instance.transform.position.z < highLightChangePos) return;

        isChangeHighLight = false;

        var changeStage = stageTileData;
        var themeColor = isHighLighted ? changeStage.highLightColor : changeStage.defaultColor;

        ChangeThemeColor(themeColor, true);
    }

    private void CheckChangeStage()
    {
        if (!isChangeStage || Player.Instance.transform.position.z < stageChangePos) return;

        isChangeStage = false;

        var changeStage = stageTileData;
        var themeColor = changeStage.defaultColor;

        ChangeThemeColor(themeColor, true);

        SoundManager.Instance.PlaySound(bgmData.bgmName, ESoundType.Bgm, 0.5f);
    }

    private void CheckChangeSpeed()
    {
        if (!isChangeSpeed || Player.Instance.transform.position.z < speedChangePos) return;

        isChangeSpeed = false;
        Player.Instance.SpeedAddValue += changeSpeedValue;
    }

    #endregion

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


    private void SetBgmData(BgmData setBgmData)
    {
        bgmData = setBgmData;
        beatInterval = 60f / bgmData.bpm * bpmDistanceMultiplier;

        isAutoTilling = bgmData.beatDataList == null || bgmData.beatDataList.Count <= 0;

        Player.Instance.SpeedAddValue = bgmData.speedAdder;
        if (isAutoTilling) return;

        if (!bgmNameToBeatData.ContainsKey(bgmData.bgmName))
        {
            bgmNameToBeatData.Add(bgmData.bgmName, new Queue<BeatData>(bgmData.beatDataList));
        }
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
}