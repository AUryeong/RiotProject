using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class LobbyUISetting : LobbyUIActiveLink
    {
        [SerializeField] private Button backgroundButton;
        [SerializeField] private InputDetail inputDetail;

        [SerializeField] private Button exitButton;

        [Header("Animation")]
        [SerializeField] private Image settingPopup;
        [SerializeField] private UIButtonColorChanger settingTitle;

        [Header("Slider")]
        [SerializeField] private UIButtonColorChanger sfxColorChanger;
        [SerializeField] private Slider sfxSlider;
        [Space(10f)]
        [SerializeField] private UIButtonColorChanger bgmColorChanger;
        [SerializeField] private Slider bgmSlider;
        [Space(10f)]
        [SerializeField] private UIButtonColorChanger syncColorChanger;
        [SerializeField] private Slider syncSlider;

        private const float UI_MOVE_DURATION = 0.5f;

        protected override void Awake()
        {
            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(DeActive);

            backgroundButton.onClick.RemoveAllListeners();
            backgroundButton.onClick.AddListener(DeActive);
            
            sfxSlider.onValueChanged.RemoveAllListeners();
            sfxSlider.onValueChanged.AddListener(SfxValueChange);
            
            sfxSlider.onValueChanged.RemoveAllListeners();
            bgmSlider.onValueChanged.AddListener(BgmValueChange);
        }

        private void SfxValueChange(float value)
        {
            SaveManager.Instance.GameData.sfxSoundMultiplier = value;
            SoundManager.Instance.UpdateVolume(ESoundType.Sfx, value);
        }

        private void BgmValueChange(float value)
        {
            SaveManager.Instance.GameData.bgmSoundMultiplier = value;
            SoundManager.Instance.UpdateVolume(ESoundType.Bgm, value);
        }

        public override void Active()
        {
            base.Active();
            
            SoundManager.Instance.PlaySound("Button", ESoundType.Sfx);
            
            backgroundButton.image.DOKill();
            backgroundButton.image.color = backgroundButton.image.color.GetAlpha(0f);
            backgroundButton.image.DOFade(0.45f, UI_MOVE_DURATION);

            settingPopup.DOKill();
            settingPopup.color = settingPopup.color.GetAlpha(0f);
            settingPopup.DOFade(1f, UI_MOVE_DURATION);

            settingTitle.RectTransform.DOKill();
            settingTitle.RectTransform.localScale = Vector3.zero;
            settingTitle.RectTransform.DOScale(Vector3.one, UI_MOVE_DURATION).SetEase(Ease.OutBack);
            
            bgmColorChanger.RectTransform.DOKill();
            bgmColorChanger.RectTransform.localScale = Vector3.zero;
            bgmColorChanger.RectTransform.DOScale(Vector3.one * 1.5f, UI_MOVE_DURATION).SetEase(Ease.OutBack);

            sfxColorChanger.RectTransform.DOKill();
            sfxColorChanger.RectTransform.localScale = Vector3.zero;
            sfxColorChanger.RectTransform.DOScale(Vector3.one * 1.5f, UI_MOVE_DURATION).SetEase(Ease.OutBack).SetDelay(UI_MOVE_DURATION / 2f);

            syncColorChanger.RectTransform.DOKill();
            syncColorChanger.RectTransform.localScale = Vector3.zero;
            syncColorChanger.RectTransform.DOScale(Vector3.one * 1.5f, UI_MOVE_DURATION).SetEase(Ease.OutBack).SetDelay(UI_MOVE_DURATION);

            var stageTileData = TileManager.Instance.stageTileData;

            settingTitle.Apply(stageTileData.uiColor, stageTileData.uiDarkColor);
            sfxColorChanger.Apply(stageTileData.uiColor, stageTileData.uiDarkColor);
            bgmColorChanger.Apply(stageTileData.uiColor, stageTileData.uiDarkColor);
            syncColorChanger.Apply(stageTileData.uiColor, stageTileData.uiDarkColor);

            inputDetail.isActivate = false;

            sfxSlider.value = SaveManager.Instance.GameData.sfxSoundMultiplier;
            bgmSlider.value = SaveManager.Instance.GameData.bgmSoundMultiplier;
            syncSlider.value = SaveManager.Instance.GameData.beatSync;
        }

        public override void DeActive()
        {
            base.DeActive();

            SoundManager.Instance.PlaySound("Button", ESoundType.Sfx);

            backgroundButton.image.DOKill(true);
            backgroundButton.image.DOFade(0, UI_MOVE_DURATION).OnComplete(() =>
            {
                gameObject.SetActive(false);
                inputDetail.isActivate = true;
            }).SetDelay(UI_MOVE_DURATION);

            settingPopup.DOKill(true);
            settingPopup.DOFade(0, UI_MOVE_DURATION);

            settingTitle.RectTransform.DOKill(true);
            settingTitle.RectTransform.DOScale(Vector3.zero, UI_MOVE_DURATION).SetEase(Ease.InBack);

            bgmColorChanger.RectTransform.DOKill(true);
            bgmColorChanger.RectTransform.DOScale(Vector3.zero, UI_MOVE_DURATION).SetEase(Ease.InBack);

            sfxColorChanger.RectTransform.DOKill(true);
            sfxColorChanger.RectTransform.DOScale(Vector3.zero, UI_MOVE_DURATION).SetEase(Ease.InBack).SetDelay(UI_MOVE_DURATION / 2f);

            syncColorChanger.RectTransform.DOKill(true);
            syncColorChanger.RectTransform.DOScale(Vector3.zero, UI_MOVE_DURATION).SetEase(Ease.InBack).SetDelay(UI_MOVE_DURATION);

            SfxValueChange(sfxSlider.value);
            BgmValueChange(bgmSlider.value);

            SaveManager.Instance.GameData.beatSync = syncSlider.value;
        }
    }
}