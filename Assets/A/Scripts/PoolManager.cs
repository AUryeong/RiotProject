using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class PoolingData
{
    [Tooltip("풀링 이름 비어있을시 오브젝트 이름으로")] public string name;
    public GameObject originObject;
    public List<GameObject> poolingList;
}

public class PoolManager : Singleton<PoolManager>
{
    private readonly Dictionary<string, List<GameObject>> pools = new();
    private readonly Dictionary<string, GameObject> originObjects = new();

    [FormerlySerializedAs("poolingDatas")] [SerializeField]
    private List<PoolingData> poolingDataList = new();

    protected override void OnCreated()
    {
        foreach (var data in poolingDataList)
        {
            string poolName = string.IsNullOrEmpty(data.name) ? data.originObject.name : data.name;
            originObjects.Add(poolName, data.originObject);

            if (data.poolingList.Count <= 0) continue;

            pools.Add(poolName, new List<GameObject>());
            foreach (var obj in data.poolingList)
            {
                pools[poolName].Add(obj);
                obj.gameObject.SetActive(false);
            }
        }

        poolingDataList.Clear();

        foreach (var stageTileData in TileManager.Instance.stageTileDataList)
        {
            foreach (var tileData in stageTileData.roadTileDataList)
            {
                string poolName = tileData.name;
                if (!originObjects.ContainsKey(poolName))
                    originObjects.Add(poolName, tileData.gameObject);
            }

            foreach (var tileData in stageTileData.outGameTileDataList)
            {
                string poolName = tileData.name;
                if (!originObjects.ContainsKey(poolName))
                    originObjects.Add(poolName, tileData.gameObject);
            }

            foreach (var tileDataList in stageTileData.tileDataList)
            {
                foreach (var tileData in tileDataList.dataList)
                {
                    string poolName = tileData.name;
                    if (!originObjects.ContainsKey(poolName))
                        originObjects.Add(poolName, tileData.gameObject);
                }
            }

            foreach (var enemy in stageTileData.flyingEnemies)
            {
                string enemyName = enemy.name;
                if (!originObjects.ContainsKey(enemyName))
                    originObjects.Add(enemyName, enemy.gameObject);
            }

            foreach (var enemy in stageTileData.defaultEnemies)
            {
                string enemyName = enemy.name;
                if (!originObjects.ContainsKey(enemyName))
                    originObjects.Add(enemyName, enemy.gameObject);
            }
        }
    }

    public GameObject Init(string origin, Transform parent = null)
    {
        if (string.IsNullOrEmpty(origin)) return null;
        if (poolingDataList.Count > 0) OnCreated();

        GameObject copy;
        if (pools.ContainsKey(origin))
        {
            if (pools[origin].FindAll((x) => !x.activeSelf).Count > 0)
            {
                copy = pools[origin].Find((x) => !x.activeSelf);
                copy.SetActive(true);

                return copy;
            }
        }
        else
        {
            pools.Add(origin, new List<GameObject>());
        }

        if (!originObjects.ContainsKey(origin))
        {
            Debug.Log(origin + " Pooling Error");
            return null;
        }

        copy = parent != null ? Instantiate(originObjects[origin], parent) : Instantiate(originObjects[origin]);
        copy.SetActive(true);

        pools[origin].Add(copy);
        return copy;
    }

    protected override void OnReset()
    {
        foreach (var obj in pools.Values.SelectMany(objs => objs))
            obj.gameObject.SetActive(false);
    }
}