using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace Edit
{
    public class EditManager : Singleton<EditManager>
    {
        private const float EDIT_TILE_DISTANCE = 0.025f;
        private const float EDIT_TILE_SCALE = 0.1f;

        [FormerlySerializedAs("plane")] [SerializeField]
        private EditGridSlot originEditGridSlot;

        [FormerlySerializedAs("layerMask")] [SerializeField]
        private LayerMask editLayerMask;

        [SerializeField] private RoadTileData roadTileData;

        [SerializeField] private EditTile[] tiles;

        private readonly List<EditGridSlot> lastTiles = new();
        private readonly List<List<EditGridSlot>> buildTiles = new();

        private int selectTileIndex;
        private int activeLine = 1;

        private float sumLength;

        private void Start()
        {
            sumLength = -3;
            CreateTile(1);
        }

        public void ClearTile()
        {
            SceneManager.LoadScene("Editor");
        }

        public void SaveTile()
        {
            roadTileData.roadDatas.Clear();

            foreach (var rowTiles in buildTiles)
            {
                var roadData = new RoadData();
                bool isJustJumping = rowTiles.FindAll(tile => tile.tileIndex != 0).Count <= 0;
                foreach (var tile in rowTiles)
                {
                    if (isJustJumping || tile.tileIndex != 0)
                        roadData.lineCondition.Add(tile.column);
                }

                roadData.length = tiles[rowTiles[0].tileIndex].length;
                roadTileData.roadDatas.Add(roadData);
            }

            roadTileData.length = roadTileData.roadDatas.Sum(roadData => roadData.length);

            string localPath = "Assets/GyungHun/TileData/Temp.prefab";

            localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

            PrefabUtility.SaveAsPrefabAssetAndConnect(roadTileData.gameObject, localPath, InteractionMode.UserAction);
        }

        public void SelectTile(int index)
        {
            float prevTileLength = tiles[selectTileIndex].length;
            float selectTileLength = tiles[index].length;

            sumLength -= prevTileLength;
            sumLength += (selectTileLength + prevTileLength) / 2;

            selectTileIndex = index;
            foreach (var editGridSlot in lastTiles)
            {
                var trans = editGridSlot.transform;

                var pos = trans.position;
                pos = new Vector3(pos.x, pos.y, sumLength);
                trans.position = pos;

                var scale = trans.localScale;
                scale = new Vector3(scale.x, scale.y, EDIT_TILE_SCALE * selectTileLength - EDIT_TILE_DISTANCE);
                trans.localScale = scale;
            }
        }

        private void CreateTile(int line)
        {
            lastTiles.Clear();
            float length = tiles[selectTileIndex].length;

            sumLength += length;
            for (int i = 0; i < 7; i++)
            {
                var editGridSlot = Instantiate(originEditGridSlot);
                editGridSlot.gameObject.SetActive(true);
                editGridSlot.row = line;
                editGridSlot.column = i - 3;

                var trans = editGridSlot.transform;
                trans.position = new Vector3((TileManager.TILE_DISTANCE * -3) + i * TileManager.TILE_DISTANCE, -2f, sumLength);
                trans.localScale = new Vector3(EDIT_TILE_SCALE * TileManager.TILE_DISTANCE - EDIT_TILE_DISTANCE, trans.localScale.y, EDIT_TILE_SCALE * length - EDIT_TILE_DISTANCE);

                lastTiles.Add(editGridSlot);
            }

            activeLine = line;
        }

        private void Update()
        {
            CheckBuild();
        }

        private void CheckBuild()
        {
            if (!Input.GetMouseButtonDown(0)) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, editLayerMask))
            {
                var buildTile = hit.collider.GetComponent<EditGridSlot>();
                if (buildTile.row < activeLine)
                {
                    if (Math.Abs(tiles[buildTiles[buildTile.row - 1][0].tileIndex].length - tiles[selectTileIndex].length) > 0.05f)
                        return;
                }

                if (buildTile.row == activeLine)
                {
                    buildTiles.Add(new List<EditGridSlot>());
                    CreateTile(buildTile.row + 1);
                }

                buildTiles[buildTile.row - 1].Add(buildTile);

                buildTile.tileIndex = selectTileIndex;
                hit.collider.gameObject.SetActive(false);

                var obj = selectTileIndex == 0 ? Instantiate(tiles[selectTileIndex].obj) : Instantiate(tiles[selectTileIndex].obj, roadTileData.transform, true);
                obj.transform.position = hit.collider.transform.position + new Vector3(0, 2f, -tiles[selectTileIndex].length / 2);
                obj.gameObject.SetActive(true);
            }
        }
    }
}