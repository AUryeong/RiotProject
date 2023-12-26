using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class PoolingData
{
    [Tooltip("풀링 이름 비어있을시 오브젝트 이름으로")] public string name;
    public int createCount = 0;
    [OnValueChanged(nameof(OnObjectChanged))] public GameObject originObject;
    public List<GameObject> poolingList;
    private void OnObjectChanged()
    {
        if (string.IsNullOrEmpty(name))
            name = originObject.name;
    }
}

public class PoolManager : Singleton<PoolManager>
{
    private readonly Dictionary<string, List<GameObject>> pools = new();
    private readonly Dictionary<string, GameObject> originObjects = new();

    [FormerlySerializedAs("poolingDatas")]
    [SerializeField]
    private List<PoolingData> poolingDataList = new();

    protected override void OnCreated()
    {
        foreach (var data in poolingDataList)
        {
            string poolName = string.IsNullOrEmpty(data.name) ? data.originObject.name : data.name;
            originObjects.Add(poolName, data.originObject);

            if (data.createCount > 0)
                CreatePoolingData(poolName, data.createCount);

            if (data.poolingList.Count <= 0) continue;

            pools.Add(poolName, new List<GameObject>());
            foreach (var obj in data.poolingList)
            {
                pools[poolName].Add(obj);
                obj.gameObject.SetActive(false);
            }
        }

        poolingDataList.Clear();
    }

    public void JoinPoolingData(string name, GameObject obj)
    {
        if (!originObjects.ContainsKey(name))
            originObjects.Add(name, obj);
    }

    public void CreatePoolingData(string origin, int count = 1, Transform parent = null)
    {
        if (string.IsNullOrEmpty(origin)) return;

        if (!originObjects.ContainsKey(origin))
        {
            Debug.Log(origin + " Pooling Error");
            return;
        }

        if (!pools.ContainsKey(origin))
            pools.Add(origin, new List<GameObject>());

        var objects = pools[origin];

        if (objects.Count > count) return;

        for (int i = 0; i < count; i++)
        {
            GameObject copy = parent != null ? Instantiate(originObjects[origin], parent) : Instantiate(originObjects[origin]);
            copy.SetActive(false);

            objects.Add(copy);
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