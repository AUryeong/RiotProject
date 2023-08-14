using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class TileManager : Singleton<TileManager>
{
    public const float TILE_DISTANCE = 2.5f;
    public const float STAGE_CHANGE_DISTANCE = 400f;

    private const float PLAYER_RENDER_DISTANCE = 70;
    public const float ENEMY_SYNC_START_POS = 3f;

    [Header("Stage")] [SerializeField] private List<StageTileData> stageTileDataList;
    [HideInInspector] public StageTileData stageTileData;

    [Header("Tiles")] private float roadTileLength;
    private readonly List<RoadTileData> createdRoadTileDataList = new();
    private List<int> lastRoadTileCondition;

    private readonly List<float> tileLengthList = new();
    private readonly List<GameObject> createdTileList = new();

    private float enemyLength;

    private float stageChangeLength;

    [Header("Bgm")] [SerializeField] private float bpmDistanceMultiplier;
    [HideInInspector] public BgmData bgmData;
    private float beatInterval;

    private bool isAutoTilling;
    private BeatData lastBeatData;

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

        stageChangeLength = STAGE_CHANGE_DISTANCE;
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

        while (enemyLength + ENEMY_SYNC_START_POS - playerPos < PLAYER_RENDER_DISTANCE)
            CheckEnemyTile(playerPos);
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
        float playerPos = Player.Instance.transform.position.z;
        CheckRoadTile(playerPos);
        CheckEnemyTile(playerPos);
        CheckBackgroundTile(playerPos);
    }

    private void CheckRoadTile(float playerPos)
    {
        if (roadTileLength - playerPos >= PLAYER_RENDER_DISTANCE + ENEMY_SYNC_START_POS) return;

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

    private void CheckEnemyTile(float playerPos)
    {
        float length = enemyLength + ENEMY_SYNC_START_POS;
        if (length - playerPos >= PLAYER_RENDER_DISTANCE) return;

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

        float playerSpeed = Player.Instance.speed + stageTileData.speedAdder;
        float beatLength = beatInterval * playerSpeed;

        if (!isAutoTilling)
        {
            var beatData = bgmNameToBeatData[bgmData.bgmName].Peek();
            while (beatData.type != BeatType.Default)
            {
                bgmNameToBeatData[bgmData.bgmName].Dequeue();
                beatData = bgmNameToBeatData[bgmData.bgmName].Peek();
            }

            var enemyNodeObj = PoolManager.Instance.Init(nameof(Enemy));
            enemyNodeObj.transform.position = new Vector3(lastRoadData.lineCondition.SelectOne() * TILE_DISTANCE, 0, length);

            enemyLength += beatLength * beatData.beatDistance;

            if (!createdTileList.Contains(enemyNodeObj))
                createdTileList.Add(enemyNodeObj);

            bgmNameToBeatData[bgmData.bgmName].Dequeue();

            if (enemyLength >= stageChangeLength)
            {
                var stageData = stageTileDataList.FindAll(data => data != stageTileData).SelectOne();
                stageTileData = stageData;

                stageChangeLength += STAGE_CHANGE_DISTANCE;

                SetBgmData(stageData.bgmDataList.SelectOne());
            }

            return;
        }

        int random = Random.Range(0, 6);

        var enemyObj = PoolManager.Instance.Init(nameof(Enemy));
        enemyObj.transform.position = new Vector3(lastRoadData.lineCondition.SelectOne() * TILE_DISTANCE, 0, length);

        if (!createdTileList.Contains(enemyObj))
            createdTileList.Add(enemyObj);

        if (random <= 2)
        {
            var enemySubObj = PoolManager.Instance.Init(nameof(Enemy));
            enemySubObj.transform.position = new Vector3(lastRoadData.lineCondition.SelectOne() * TILE_DISTANCE, 0, length + beatLength / 2);

            if (!createdTileList.Contains(enemySubObj))
                createdTileList.Add(enemySubObj);
            enemyLength += beatLength;
        }

        if (random <= 0)
        {
            var enemySubObj = PoolManager.Instance.Init(nameof(Enemy));
            enemySubObj.transform.position = new Vector3(lastRoadData.lineCondition.SelectOne() * TILE_DISTANCE, 0, length + beatLength);

            if (!createdTileList.Contains(enemySubObj))
                createdTileList.Add(enemySubObj);
            enemyLength += beatLength;
        }

        enemyLength += beatLength * 2;
        if (enemyLength >= stageChangeLength)
        {
            var stageData = stageTileDataList.FindAll(data => data != stageTileData).SelectOne();
            stageTileData = stageData;

            stageChangeLength += STAGE_CHANGE_DISTANCE;

            SetBgmData(stageData.bgmDataList.SelectOne());
        }
    }

    private void SetBgmData(BgmData setBgmData)
    {
        bgmData = setBgmData;
        beatInterval = 60f / bgmData.bpm * bpmDistanceMultiplier;

        isAutoTilling = bgmData.beatDataList == null || bgmData.beatDataList.Count <= 0;

        float playerSpeed = Player.Instance.speed + stageTileData.speedAdder;
        
        if (!isAutoTilling)
        {
            if (!bgmNameToBeatData.ContainsKey(bgmData.bgmName))
            {
                bgmNameToBeatData.Add(bgmData.bgmName, new Queue<BeatData>(bgmData.beatDataList));
            }
            var beatData = bgmNameToBeatData[bgmData.bgmName].Dequeue();
            enemyLength += beatData.beatDistance * beatInterval * playerSpeed;

            InGameManager.Instance.ChangeStage(enemyLength, (beatData.beat - bgmData.beatDataList.Peek().beat) * beatInterval);
            return;
        }

        enemyLength += 2 * beatInterval * playerSpeed;
        InGameManager.Instance.ChangeStage(enemyLength, 0);
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