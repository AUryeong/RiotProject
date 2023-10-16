using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class RoadTileData : MonoBehaviour
{
    public List<RoadData> roadDatas;
    public List<GameObject> roadObjects;
    public float length;

    private void OnEnable()
    {
        if (!GameManager.Instance.isGaming) return;

        float playerPos = Player.Instance.transform.position.x;

        foreach (var obj in roadObjects)
        {
            obj.transform.DOKill(true);
            obj.transform.localPosition -= new Vector3(0, 6, 0);
            obj.transform.DOLocalMoveY(0, 0.5f).SetDelay(0.5f * Mathf.Abs(playerPos - obj.transform.position.x) / TileManager.TILE_DISTANCE);
        }
    }
}

[System.Serializable]
public class RoadData
{
    public float length;

    [HideInInspector] public int summonLine;

    public List<int> lineCondition = new();
    public bool isJustBlank;
}