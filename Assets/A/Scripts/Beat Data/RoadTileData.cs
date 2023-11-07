using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class RoadTileData : TileData
{
    [HideInInspector] public int summonLine;

    public List<GameObject> roadObjects;

    private void OnEnable()
    {
        float playerPos = Player.Instance.transform.position.x;

        float beatInterval = TileManager.Instance.beatInterval;
        Vector3 localPos = new Vector3(0, 6, 0);

        foreach (var obj in roadObjects)
        {
            obj.transform.DOKill(true);
            obj.transform.localPosition -= localPos;
            obj.transform.DOLocalMoveY(0, beatInterval).SetDelay(beatInterval * 0.5f * Mathf.Abs(playerPos - obj.transform.position.x) / TileManager.TILE_DISTANCE);
        }
    }
}