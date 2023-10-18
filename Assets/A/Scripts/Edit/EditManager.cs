using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Edit
{
    public class EditManager : Singleton<EditManager>
    {
        private const float EDIT_TILE_DISTANCE = 0.025f;
        private const float EDIT_TILE_SCALE = 0.1f;

        [SerializeField] private EditGridSlot originEditGridSlot;

        [SerializeField] private LayerMask editLayerMask;

        [SerializeField] private RoadTileData roadTileData;
        [SerializeField] private EditStageTile stageTile;
        [SerializeField] private GlobalObjectFogController controller;

        [Space(10)]
        [SerializeField] private RectTransform tileSelectParent;
        [SerializeField] private Button tileSelectButton;

        private readonly List<EditGridSlot> lastTiles = new();
        private readonly List<EditGridSlot> buildTiles = new();

        private int selectTileIndex;

        private float sumLength;

        private void Start()
        {
            sumLength = -3;
            controller.mainColor = stageTile.tileData.defaultColor.mainColor;
            controller.fogColor = stageTile.tileData.defaultColor.fogColor;

            for (int i = 0; i < stageTile.tiles.Count; i++)
            {
                var obj = Instantiate(tileSelectButton, tileSelectParent);

                int temp = i;
                obj.gameObject.SetActive(true);

                obj.onClick.RemoveAllListeners();
                obj.onClick.AddListener(() => SelectTile(temp));

                obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (stageTile.tiles[temp].isBlank ? "B_" : "") + stageTile.tiles[temp].objects[0].name;
            }

            CreateTile();
        }

        public void ClearTile()
        {
            SceneManager.LoadScene("Editor");
        }

#if UNITY_EDITOR
        public void SaveTile()
        {
            foreach (var tile in buildTiles)
            {
                if (!tile.isBlank)
                    roadTileData.lineCondition.Add(tile.column);
            }

            roadTileData.length = stageTile.tiles[buildTiles[0].tileIndex].length;

            string localPath = "Assets/GyungHun/TileData/Temp.prefab";

            localPath = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(localPath);

            UnityEditor.PrefabUtility.SaveAsPrefabAssetAndConnect(roadTileData.gameObject, localPath, UnityEditor.InteractionMode.UserAction);
        }
#endif

        public void SelectTile(int index)
        {
            float prevTileLength = stageTile.tiles[selectTileIndex].length;
            float selectTileLength = stageTile.tiles[index].length;

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

        private void CreateTile()
        {
            lastTiles.Clear();
            float length = stageTile.tiles[selectTileIndex].length;

            sumLength += length;
            for (int i = 0; i < 7; i++)
            {
                var editGridSlot = Instantiate(originEditGridSlot);
                editGridSlot.gameObject.SetActive(true);
                editGridSlot.column = i - 3;

                var trans = editGridSlot.transform;
                trans.position = new Vector3((TileManager.TILE_DISTANCE * -3) + i * TileManager.TILE_DISTANCE, -2f, sumLength);
                trans.localScale = new Vector3(EDIT_TILE_SCALE * TileManager.TILE_DISTANCE - EDIT_TILE_DISTANCE, trans.localScale.y, EDIT_TILE_SCALE * length - EDIT_TILE_DISTANCE);

                lastTiles.Add(editGridSlot);
            }
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
                if (buildTiles.Count > 0)
                    if (Math.Abs(stageTile.tiles[buildTiles[0].tileIndex].length - stageTile.tiles[selectTileIndex].length) > 0.05f)
                        return;

                buildTiles.Add(buildTile);

                buildTile.tileIndex = selectTileIndex;

                var tileData = stageTile.tiles[selectTileIndex];
                buildTile.isBlank = tileData.isBlank;
                roadTileData.length = tileData.length;

                hit.collider.gameObject.SetActive(false);

                var obj = Instantiate(tileData.objects.SelectOne(), roadTileData.transform, true);
                obj.transform.position = hit.collider.transform.position + new Vector3(0, 2f, -stageTile.tiles[selectTileIndex].length / 2);
                obj.gameObject.SetActive(true);

                roadTileData.roadObjects.Add(obj);
            }
        }
    }
}