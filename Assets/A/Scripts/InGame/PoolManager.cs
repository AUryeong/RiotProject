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

    [FormerlySerializedAs("poolingDatas")] [SerializeField] private List<PoolingData> poolingDataList = new();
    [FormerlySerializedAs("stageTileDatas")] [SerializeField] private List<StageTileData> stageTileDataList = new();

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

        foreach (var stageTileData in stageTileDataList)
        {
            foreach (var tileData in stageTileData.roadTileDataList)
            {
                string poolName = tileData.name;
                originObjects.Add(poolName, tileData.gameObject);
            }
            foreach (var tileDataList in stageTileData.tileDataList)
            {
                foreach (var tileData in tileDataList.dataList)
                {
                    string poolName = tileData.name;
                    originObjects.Add(poolName, tileData.gameObject);
                }
            }
        }
    }

    public GameObject Init(string origin)
    {
        if (string.IsNullOrEmpty(origin)) return null;

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
            Debug.Log("풀링 에러");
            return null;
        }

        copy = Instantiate(originObjects[origin]);
        copy.SetActive(true);
        DontDestroyOnLoad(copy);

        pools[origin].Add(copy);
        return copy;
    }

    protected override void OnReset()
    {
        foreach (var obj in pools.Values.SelectMany(objs => objs))
            obj.gameObject.SetActive(false);
    }
}