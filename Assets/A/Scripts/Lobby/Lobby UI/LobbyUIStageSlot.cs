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
        }
    }
}