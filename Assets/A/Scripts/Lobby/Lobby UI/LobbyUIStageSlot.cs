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

        [SerializeField] private List<Image> bgmSlots;
        [SerializeField] private List<TextMeshProUGUI> bgmSlotTexts;

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
            nickNameText.text = stageTileData.stageNickName;

            for (int i = 0; i < bgmSlots.Count; i++)
            {
                if (stageTileData.bgmDataList.Count > i)
                {
                    bgmSlots[i].gameObject.SetActive(true);
                    bgmSlotTexts[i].text = stageTileData.bgmDataList[i].bgmNickName;
                }
                else
                {
                    bgmSlots[i].gameObject.SetActive(false);
                }
            }
        }
    }
}