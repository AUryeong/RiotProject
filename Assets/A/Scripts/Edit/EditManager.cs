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
        private readonly List<List<EditGridSlot>> buildTiles = new();

        private int selectTileIndex;
        private int activeLine = 1;

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
                obj.onClick.AddListener(()=>SelectTile(temp));
                
                obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = stageTile.tiles[temp].objects[0].name;
            }
            
            CreateTile(1);
        }

        public void ClearTile()
        {
            SceneManager.LoadScene("Editor");
        }

#if UNITY_EDITOR
        public void SaveTile()
        {
            roadTileData.roadDatas.Clear();

            foreach (var rowTiles in buildTiles)
            {
                var roadData = new RoadData();
                roadData.isJustBlank = rowTiles.FindAll(tile => !tile.isBlank).Count <= 0;
                foreach (var tile in rowTiles)
                {
                    if (roadData.isJustBlank || !tile.isBlank)
                        roadData.lineCondition.Add(tile.column);
                }

                roadData.length = stageTile.tiles[rowTiles[0].tileIndex].length;
            }

            roadTileData.length = roadTileData.roadDatas.Sum(roadData => roadData.length);

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

        private void CreateTile(int line)
        {
            lastTiles.Clear();
            float length = stageTile.tiles[selectTileIndex].length;

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
                    if (Math.Abs(stageTile.tiles[buildTiles[buildTile.row - 1][0].tileIndex].length - stageTile.tiles[selectTileIndex].length) > 0.05f)
                        return;
                }

                if (buildTile.row == activeLine)
                {
                    buildTiles.Add(new List<EditGridSlot>());
                    CreateTile(buildTile.row + 1);
                }

                buildTiles[buildTile.row - 1].Add(buildTile);

                buildTile.tileIndex = selectTileIndex;

                var tileData = stageTile.tiles[selectTileIndex];
                buildTile.isBlank = tileData.isBlank;
                
                hit.collider.gameObject.SetActive(false);

                var obj = buildTile.isBlank ? Instantiate(tileData.objects.SelectOne()) : Instantiate(tileData.objects.SelectOne(), roadTileData.transform, true);
                obj.transform.position = hit.collider.transform.position + new Vector3(0, 2f, -stageTile.tiles[selectTileIndex].length / 2);
                obj.gameObject.SetActive(true);
               
                roadTileData.roadObjects.Add(obj);
            }
        }
    }
}