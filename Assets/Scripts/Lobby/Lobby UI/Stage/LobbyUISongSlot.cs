using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class LobbyUISongSlot : MonoBehaviour
    {
        [SerializeField] private RectTransform iconParent;
        [SerializeField] private Image songIcon;
        [SerializeField] private Image songIconGradient;
        [SerializeField] private TextMeshProUGUI songText;

        [Header("Lock")]
        [SerializeField] private Image lockBackground;
        [SerializeField] private Image runeIcon;
        [SerializeField] private TextMeshProUGUI runeText;

        private Button button;
        
        private BgmData data;
        private StageData stageData;

        private void Awake()
        {
            button = GetComponent<Button>();

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            transform.DOKill(true);
            if (stageData.isBuy)
            {
                transform.DOPunchScale(Vector3.one * 0.1f, 0.3f);
                LobbyManager.Instance.uiManager.uiStage.SelectBgm(data);
            }
            else
            {
                if (SaveManager.Instance.GameData.rune >= data.price)
                {
                    stageData.isBuy = true;
                    SaveManager.Instance.GameData.rune -= data.price;
                    
                    lockBackground.DOFade(0, 0.5f);
                    
                    runeIcon.DOFade(0, 0.5f);
                    runeIcon.rectTransform.DORotate(new Vector3(0, 0, 720), 0.5f, RotateMode.FastBeyond360);
                    runeIcon.rectTransform.DOScale(Vector3.one * 2, 0.5f);
                    
                    runeText.DOFade(0, 0.5f);
                    runeText.rectTransform.DOScale(Vector3.one * 2, 0.5f).OnComplete(() =>
                    {
                        lockBackground.gameObject.SetActive(false);
                        runeIcon.gameObject.SetActive(false);
                        runeText.gameObject.SetActive(false);
                    });

                    runeText.rectTransform.position = Vector3.zero;
                    
                    var characterInfo = songText.textInfo.characterInfo[0];
                    iconParent.localPosition = (characterInfo.topLeft + characterInfo.bottomLeft) / 2 + new Vector3(-30, 0, 0);
                    
                    songIconGradient.gameObject.SetActive(stageData.isBuy);
                    lockBackground.gameObject.SetActive(!stageData.isBuy);
                    
                    transform.DOPunchScale(Vector3.one * 0.1f, 0.3f);
                    SoundManager.Instance.PlaySound("levelup", ESoundType.Sfx, 0.6f);
                    LobbyManager.Instance.uiManager.uiStage.SelectBgm(data);
                }
                else
                {
                    SoundManager.Instance.PlaySound("Error", ESoundType.Sfx, 0.6f);
                }

            }
        }

        public void Show(BgmData bgmData, StageTileData stageTileData)
        {
            if (songText.textInfo == null) return;

            transform.DOKill(true);

            int stageIndex = TileManager.Instance.stageTileDataList.IndexOf(stageTileData);
            int bgmIndex = TileManager.Instance.stageTileDataList[stageIndex].bgmDataList.IndexOf(bgmData);

            data = bgmData;
            stageData = SaveManager.Instance.GameData.GetStageData(stageIndex, bgmIndex);

            songText.text = bgmData.bgmNickName;

            runeIcon.DOKill();
            runeIcon.color = runeIcon.color.GetAlpha(1);
            runeIcon.rectTransform.rotation = Quaternion.identity;
            runeIcon.rectTransform.localScale = Vector3.one * 0.6f;

            runeText.DOKill();
            runeText.color = runeText.color.GetAlpha(1);
            runeText.rectTransform.localScale = Vector3.one;
            runeText.text = bgmData.price.ToString();

            songText.ForceMeshUpdate(true);
            runeText.ForceMeshUpdate(true);
            
            var characterInfo = !stageData.isBuy ? runeText.textInfo.characterInfo[0] : songText.textInfo.characterInfo[0];
            iconParent.localPosition = (characterInfo.topLeft + characterInfo.bottomLeft) / 2 + new Vector3(-30, 0, 0);

            songText.fontMaterial.SetColor("_OutlineColor", stageTileData.uiDarkColor);
            runeText.fontMaterial.SetColor("_OutlineColor", stageTileData.uiDarkColor);
            songIcon.color = stageTileData.uiColor;

            songIconGradient.gameObject.SetActive(stageData.isBuy);
            lockBackground.gameObject.SetActive(!stageData.isBuy);
            runeIcon.gameObject.SetActive(!stageData.isBuy);
        }
    }
}