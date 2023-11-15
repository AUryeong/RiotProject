using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class LobbyUIStageSlot : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI nickNameText;
        [SerializeField] private LobbyUISongSlot[] slots;

        public RectTransform RectTransform
        {
            get
            {
                if (rectTransfrom == null)
                    rectTransfrom = GetComponent<RectTransform>();
                return rectTransfrom;
            }
        }

        private RectTransform rectTransfrom;

        public void ShowStage(StageTileData stageTileData)
        {
            levelText.text = stageTileData.name;
            levelText.fontMaterial.SetColor("_OutlineColor", stageTileData.uiDarkColor);

            nickNameText.text = stageTileData.stageNickName;
            nickNameText.fontMaterial.SetColor("_OutlineColor", stageTileData.uiDarkColor);

            for (int i = 0; i < slots.Length; i++)
            {
                LobbyUISongSlot slot = slots[i];
                var bgmData = stageTileData.bgmDataList[i];
                slot.Show(bgmData, stageTileData);
            }
        }
    }
}