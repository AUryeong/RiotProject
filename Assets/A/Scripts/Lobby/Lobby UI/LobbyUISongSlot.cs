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

        public void Show(BgmData bgmData, StageTileData stageTileData)
        {
            if (songText.textInfo == null) return;

            songText.text = bgmData.bgmNickName;

            songText.ForceMeshUpdate(true);
            var characterInfo = songText.textInfo.characterInfo[0];
            songIconGraident.rectTransform.localPosition = (characterInfo.topLeft + characterInfo.bottomLeft) / 2 + new Vector3(-30, 0, 0);

            songText.fontMaterial.SetColor("_OutlineColor", stageTileData.uiDarkColor);
            songIcon.color = stageTileData.uiColor;
        }
    }
}