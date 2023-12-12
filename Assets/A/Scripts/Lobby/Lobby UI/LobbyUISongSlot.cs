using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class LobbyUISongSlot : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI songText;
        [SerializeField] private Image songIconGraident; 
        [SerializeField] private Image songIcon;

        private Button button;
        private BgmData data;

        private void Awake()
        {
            button = GetComponent<Button>();
            
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(SelectBgm);
        }

        private void SelectBgm()
        {
            transform.DOPunchScale(Vector3.one * 0.1f, 0.3f);
            LobbyManager.Instance.uiManager.uiStage.SelectBgm(data);
        }

        public void Show(BgmData bgmData, StageTileData stageTileData)
        {
            if (songText.textInfo == null) return;

            data = bgmData;

            songText.text = bgmData.bgmNickName;

            songText.ForceMeshUpdate(true);
            var characterInfo = songText.textInfo.characterInfo[0];
            songIconGraident.rectTransform.localPosition = (characterInfo.topLeft + characterInfo.bottomLeft) / 2 + new Vector3(-30, 0, 0);

            songText.fontMaterial.SetColor("_OutlineColor", stageTileData.uiDarkColor);
            songIcon.color = stageTileData.uiColor;
        }
    }
}