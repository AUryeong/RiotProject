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
                if (rectTransform == null)
                    rectTransform = transform as RectTransform;
                return rectTransform;
            }
        }

        private RectTransform rectTransform;

        public void ShowStage(StageTileData stageTileData)
        {
            levelText.rectTransform.DOKill(true);
            levelText.text = stageTileData.name;
            levelText.fontMaterial.SetColor("_OutlineColor", stageTileData.uiDarkColor);

            nickNameText.rectTransform.DOKill(true);
            nickNameText.text = stageTileData.stageNickName;
            nickNameText.fontMaterial.SetColor("_OutlineColor", stageTileData.uiDarkColor);

            for (int i = 0; i < slots.Length; i++)
            {
                LobbyUISongSlot slot = slots[i];
                slots[i].transform.DOKill(true);
                var bgmData = stageTileData.bgmDataList[i];
                slot.Show(bgmData, stageTileData);
            }
        }

        public void Bounce(int bgmSelectIndex)
        {
            float duration = TileManager.Instance.beatInterval / 4;

            Vector3 scale = Vector3.one * 1.1f;

            levelText.rectTransform.DOKill(true);
            levelText.rectTransform.DOScale(scale, duration).SetLoops(2, LoopType.Yoyo);

            nickNameText.rectTransform.DOKill(true);
            nickNameText.rectTransform.DOScale(scale, duration).SetLoops(2, LoopType.Yoyo);

            slots[bgmSelectIndex].transform.DOKill(true);
            slots[bgmSelectIndex].transform.DOScale(scale, duration).SetLoops(2, LoopType.Yoyo);
        }
    }
}