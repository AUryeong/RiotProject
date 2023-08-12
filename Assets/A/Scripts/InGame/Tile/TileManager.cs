using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileManager : Singleton<TileManager>
{
    private const float PLAYER_RENDER_DISTANCE = 70;
    public const float TILE_DISTANCE = 2.5f;
    private const float SYNC_DISTANCE = 0.1f;

    public StageTileData stageTileData;

    private float roadTileLength;

    private List<RoadTileData> createdRoadTileDatas = new();
    private List<int> lastRoadTileCondition;

    private readonly List<float> tileLength = new();
    private readonly List<GameObject> createdTileList = new();

    private float enemyLength;
    public int bpm;
    public float speed;
    private float beatInterval;

    protected override void OnCreated()
    {
        base.OnCreated();
        beatInterval = 60f / bpm * speed;
        enemyLength = beatInterval * 2 * Player.Instance.speed;
        roadTileLength = 0;
        for (int index = 0; index < stageTileData.tileDataList.Count; index++)
            tileLength.Add(0);
    }

    private void Start()
    {
        CreateTile();
    }

    private void CreateTile()
    {
        var data = stageTileData.roadTileDataList[0];

        var obj = PoolManager.Instance.Init(data.name);
        obj.transform.position = new Vector3(0, 0, roadTileLength);
        createdTileList.Add(obj);

        createdRoadTileDatas.Add(data);
        lastRoadTileCondition = data.roadDatas[^1].lineCondition;
        roadTileLength += data.length;

        float playerPos = Player.Instance.transform.position.z;
        while (roadTileLength - playerPos < PLAYER_RENDER_DISTANCE)
            CheckRoadTile(playerPos);

        while (enemyLength - playerPos < PLAYER_RENDER_DISTANCE)
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

        foreach (var obj in createdTileList)
        {
            if (!obj.gameObject.activeSelf) continue;
            if (playerPos - obj.transform.position.z > PLAYER_RENDER_DISTANCE)
                obj.gameObject.SetActive(false);
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
        if (roadTileLength - playerPos >= PLAYER_RENDER_DISTANCE) return;

        var data = stageTileData.roadTileDataList.FindAll(tileData => tileData.roadDatas[0].lineCondition.Intersect(lastRoadTileCondition).Any()).SelectOne();

        var obj = PoolManager.Instance.Init(data.name);
        obj.transform.position = new Vector3(0, 0, roadTileLength);

        if (!createdTileList.Contains(obj))
            createdTileList.Add(obj);

        if (createdTileList.Contains(obj))
            createdRoadTileDatas.Remove(data);

        createdRoadTileDatas.Add(data);
        lastRoadTileCondition = data.roadDatas[^1].lineCondition;
        roadTileLength += data.length;
    }

    private void CheckEnemyTile(float playerPos)
    {
        if (enemyLength - playerPos >= PLAYER_RENDER_DISTANCE) return;

        float roadLength = roadTileLength;
        int lastIndex = createdRoadTileDatas.Count;
        while (roadLength > enemyLength)
        {
            lastIndex--;
            roadLength -= createdRoadTileDatas[lastIndex].length;
        }

        float distance = enemyLength - roadTileLength;
        RoadData lastRoadData = null;
        foreach (var data in createdRoadTileDatas[lastIndex].roadDatas)
        {
            distance += data.length;
            if (distance > enemyLength)
                return;
            lastRoadData = data;
        }

        int random = Random.Range(0, 5);

        float beatLength = beatInterval * (InGameManager.Instance.isAlice ? Player.Instance.speed + 10 : Player.Instance.speed);

        if (random <= 4)
        {
            var obj = PoolManager.Instance.Init(nameof(Enemy));
            obj.transform.position = new Vector3(lastRoadData.lineCondition.SelectOne() * TILE_DISTANCE, 0, enemyLength + SYNC_DISTANCE);

            if (!createdTileList.Contains(obj))
                createdTileList.Add(obj);
        }

        if (random <= 2)
        {
            var obj = PoolManager.Instance.Init(nameof(Enemy));
            obj.transform.position = new Vector3(lastRoadData.lineCondition.SelectOne() * TILE_DISTANCE, 0, enemyLength + SYNC_DISTANCE + beatLength / 8);

            if (!createdTileList.Contains(obj))
                createdTileList.Add(obj);
            enemyLength += beatLength / 2;
        }

        if (random <= 0)
        {
            var obj = PoolManager.Instance.Init(nameof(Enemy));
            obj.transform.position = new Vector3(lastRoadData.lineCondition.SelectOne() * TILE_DISTANCE, 0, enemyLength + SYNC_DISTANCE + beatLength / 4);

            if (!createdTileList.Contains(obj))
                createdTileList.Add(obj);
            enemyLength += beatLength / 2;
        }


        enemyLength += beatLength;
        if (!InGameManager.Instance.isAlice && enemyLength >= 400)
        {
            InGameManager.Instance.isAlice = true;
            InGameManager.Instance.alice = enemyLength;
            bpm = 155;
            beatInterval = 60f / bpm * speed;
        }
    }

    private void CheckBackgroundTile(float playerPos)
    {
        for (var index = 0; index < stageTileData.tileDataList.Count; index++)
        {
            if (tileLength[index] - playerPos >= PLAYER_RENDER_DISTANCE) continue;

            var stage = stageTileData.tileDataList[index];
            var data = stage.dataList.SelectOne();

            var obj = PoolManager.Instance.Init(data.name);
            obj.transform.position = new Vector3(0, 0, tileLength[index]);

            if (!createdTileList.Contains(obj))
                createdTileList.Add(obj);

            tileLength[index] += data.length;
        }
    }
}