using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class LobbyUIStage : LobbyUIActiveLink
    {
        [SerializeField] private InputDetail inputDetail;

        [Space(10f)]

        [SerializeField] private LobbyUIStageSlot mainStageSlot;
        [SerializeField] private LobbyUIStageSlot sideStageSlot;

        [SerializeField] private Button selectButton;
        [SerializeField] private Image borderBackground;

        [Space(10f)]

        [SerializeField] private Button exitButton;

        [Space(10f)]

        [SerializeField] private Image stageSelectLeft;
        [SerializeField] private Image stageSelectRight;

        [Space(10f)]

        [SerializeField] private UIButtonColorChanger changeButtonChanger;

        private int stageSelectIndex;
        private int bgmSelectIndex;

        private const float UI_MOVE_DURATION = 0.75f;
        private const float UI_DRAG_MOVE_DURATION = 0.5f;

        private bool isDeActivating;

        [Header("Animation")]
        private Sequence deActiveSequence;
        private Sequence activeSequence;
        protected override void Awake()
        {
            inputDetail.inputAction = CheckInput;

            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(SelectButton);

            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(ExitButton);
        }

        private void ExitButton()
        {
            if (isDeActivating) return;

            isDeActivating = true;

            SoundManager.Instance.PlaySound("Button", ESoundType.Sfx);

            LobbyManager.Instance.uiManager.Select(LobbyType.Home);
        }

        private void SelectButton()
        {
            if (isDeActivating) return;

            isDeActivating = true;

            SoundManager.Instance.PlaySound("levelup", ESoundType.Sfx, 0.6f, 0.9f);

            SaveManager.Instance.GameData.selectStageIndex = stageSelectIndex;
            SaveManager.Instance.GameData.selectBgmIndex = bgmSelectIndex;

            GameManager.Instance.ActiveSceneLink(SceneLinkType.Lobby);
        }

        public void SelectBgm(BgmData bgmData)
        {
            SoundManager.Instance.PlaySound("levelup_back", ESoundType.Sfx, 0.6f, 0.9f);

            var stageTileData = TileManager.Instance.stageTileDataList[stageSelectIndex];
            bgmSelectIndex = stageTileData.bgmDataList.IndexOf(bgmData);
            TileManager.Instance.SetBgmData(bgmData);
        }

        public override void Active()
        {
            base.Active();
            isDeActivating = false;
            stageSelectIndex = SaveManager.Instance.GameData.selectStageIndex;
            inputDetail.gameObject.SetActive(true);

            StageTileData stageTileData = TileManager.Instance.stageTileDataList[stageSelectIndex];
            mainStageSlot.ShowStage(stageTileData);
            sideStageSlot.ShowStage(stageTileData);

            changeButtonChanger.Apply(stageTileData.uiColor, stageTileData.uiDarkColor);

            borderBackground.color = stageTileData.uiDarkColor.GetAlpha(0.35f);

            activeSequence?.Complete();
            deActiveSequence?.Complete();

            if (activeSequence != null)
            {
                activeSequence.Restart();
                return;
            }
            activeSequence = DOTween.Sequence();

            activeSequence.SetAutoKill(false);
            activeSequence.OnStart(() =>
            {
                sideStageSlot.ShowStage(TileManager.Instance.stageTileDataList[SaveManager.Instance.GameData.selectStageIndex]);
                selectButton.image.rectTransform.localScale = Vector3.zero;
                sideStageSlot.RectTransform.localScale = Vector3.zero;
                borderBackground.color = borderBackground.color.GetAlpha(1);
                stageSelectLeft.color = stageSelectLeft.color.GetAlpha(1);
                stageSelectRight.color = stageSelectRight.color.GetAlpha(1);
                exitButton.image.rectTransform.anchoredPosition = new Vector2(-70, exitButton.image.rectTransform.anchoredPosition.y);
            });

            activeSequence.Join(sideStageSlot.RectTransform.DOScale(Vector3.one, UI_MOVE_DURATION).SetEase(Ease.OutBack));
            activeSequence.Join(selectButton.image.rectTransform.DOScale(Vector3.one, UI_MOVE_DURATION).SetEase(Ease.OutBack));
            activeSequence.Join(exitButton.image.rectTransform.DOAnchorPosX(70, UI_MOVE_DURATION));
            activeSequence.Join(borderBackground.DOFade(1, UI_MOVE_DURATION));
            activeSequence.Join(stageSelectRight.DOFade(1, UI_MOVE_DURATION));
            activeSequence.Join(stageSelectLeft.DOFade(1, UI_MOVE_DURATION));

            activeSequence.OnUpdate(() =>
            {
                if (!Input.GetMouseButtonDown(0)) return;
                activeSequence.Complete(true);
            });
        }

        public override void DeActive()
        {
            base.DeActive();
            inputDetail.gameObject.SetActive(false);

            var bgmData = TileManager.Instance.stageTileData.bgmDataList[SaveManager.Instance.GameData.selectBgmIndex];
            TileManager.Instance.SetBgmData(bgmData);

            activeSequence?.Complete(true);
            deActiveSequence?.Complete(true);

            if (deActiveSequence != null)
            {
                deActiveSequence.Restart();
                return;
            }
            deActiveSequence = DOTween.Sequence();

            deActiveSequence.SetAutoKill(false);
            deActiveSequence.OnStart(() =>
            {
                sideStageSlot.RectTransform.localScale = Vector3.one;
                selectButton.image.rectTransform.localScale = Vector3.one;
                exitButton.image.rectTransform.anchoredPosition = new Vector2(70, exitButton.image.rectTransform.anchoredPosition.y);
                borderBackground.color = borderBackground.color.GetAlpha(0);
                stageSelectLeft.color = stageSelectLeft.color.GetAlpha(0);
                stageSelectRight.color = stageSelectRight.color.GetAlpha(0);
            });

            deActiveSequence.Join(sideStageSlot.RectTransform.DOScale(Vector3.zero, UI_MOVE_DURATION / 2).SetEase(Ease.InBack));
            deActiveSequence.Join(selectButton.image.rectTransform.DOScale(Vector3.zero, UI_MOVE_DURATION / 2).SetEase(Ease.InBack));
            deActiveSequence.Join(exitButton.image.rectTransform.DOAnchorPosX(-70, UI_MOVE_DURATION / 2));
            deActiveSequence.Join(borderBackground.DOFade(1, UI_MOVE_DURATION / 2));
            deActiveSequence.Join(stageSelectLeft.DOFade(1, UI_MOVE_DURATION / 2));
            deActiveSequence.Join(stageSelectRight.DOFade(1, UI_MOVE_DURATION / 2));

            deActiveSequence.OnComplete(() => gameObject.SetActive(false));
        }

        private void CheckInput(Direction direction)
        {
            if (direction == Direction.Left)
                PrevStage();
            else
                NextStage();
        }

        private void NextStage()
        {
            SoundManager.Instance.PlaySound("levelup_back", ESoundType.Sfx, 0.6f, 0.9f);

            mainStageSlot.RectTransform.DOKill(true);
            sideStageSlot.RectTransform.DOKill(true);
            stageSelectRight.rectTransform.DOKill(true);

            stageSelectIndex = stageSelectIndex - 1 < 0 ? TileManager.Instance.stageTileDataList.Count - 1 : stageSelectIndex - 1;

            var stageTileData = TileManager.Instance.stageTileDataList[stageSelectIndex];

            mainStageSlot.gameObject.SetActive(true);
            mainStageSlot.RectTransform.anchoredPosition = new Vector2(-GameManager.Instance.ScreenSize.x, mainStageSlot.RectTransform.anchoredPosition.y);
            mainStageSlot.ShowStage(stageTileData);

            changeButtonChanger.ApplyFade(UI_DRAG_MOVE_DURATION, stageTileData.uiColor, stageTileData.uiDarkColor);

            borderBackground.DOKill(true);
            borderBackground.DOColor(stageTileData.uiDarkColor.GetAlpha(0.35f), UI_DRAG_MOVE_DURATION);

            stageSelectRight.rectTransform.DOPunchScale(Vector3.one * 0.6f, UI_DRAG_MOVE_DURATION);
            sideStageSlot.RectTransform.DOAnchorPosX(GameManager.Instance.ScreenSize.x, UI_DRAG_MOVE_DURATION).SetEase(Ease.OutBack);
            mainStageSlot.RectTransform.DOAnchorPosX(0, UI_DRAG_MOVE_DURATION).SetEase(Ease.OutBack).OnComplete(() =>
            {
                sideStageSlot.RectTransform.anchoredPosition = new Vector2(0, sideStageSlot.RectTransform.anchoredPosition.y);
                sideStageSlot.ShowStage(stageTileData);
                mainStageSlot.gameObject.SetActive(false);

                bgmSelectIndex = Random.Range(0, stageTileData.bgmDataList.Count);
                TileManager.Instance.SetBgmData(stageTileData.bgmDataList[bgmSelectIndex]);
            });
        }

        private void PrevStage()
        {
            SoundManager.Instance.PlaySound("levelup_back", ESoundType.Sfx, 0.6f, 0.9f);

            mainStageSlot.RectTransform.DOKill(true);
            sideStageSlot.RectTransform.DOKill(true);
            stageSelectLeft.rectTransform.DOKill(true);

            stageSelectIndex = (stageSelectIndex + 1) % TileManager.Instance.stageTileDataList.Count;

            var stageTileData = TileManager.Instance.stageTileDataList[stageSelectIndex];

            mainStageSlot.gameObject.SetActive(true);
            mainStageSlot.RectTransform.anchoredPosition = new Vector2(GameManager.Instance.ScreenSize.x, mainStageSlot.RectTransform.anchoredPosition.y);
            mainStageSlot.ShowStage(stageTileData);

            changeButtonChanger.ApplyFade(UI_DRAG_MOVE_DURATION, stageTileData.uiColor, stageTileData.uiDarkColor);

            borderBackground.DOKill(true);
            borderBackground.DOColor(stageTileData.uiDarkColor.GetAlpha(0.35f), UI_DRAG_MOVE_DURATION);

            stageSelectLeft.rectTransform.DOPunchScale(Vector3.one * 0.6f, UI_DRAG_MOVE_DURATION);
            sideStageSlot.RectTransform.DOAnchorPosX(-GameManager.Instance.ScreenSize.x, UI_DRAG_MOVE_DURATION).SetEase(Ease.OutBack);
            mainStageSlot.RectTransform.DOAnchorPosX(0, UI_DRAG_MOVE_DURATION).SetEase(Ease.OutBack).OnComplete(() =>
            {
                sideStageSlot.RectTransform.anchoredPosition = new Vector2(0, sideStageSlot.RectTransform.anchoredPosition.y);
                sideStageSlot.ShowStage(stageTileData);
                mainStageSlot.gameObject.SetActive(false);

                bgmSelectIndex = Random.Range(0, stageTileData.bgmDataList.Count);
                TileManager.Instance.SetBgmData(stageTileData.bgmDataList[bgmSelectIndex]);
            });
        }

        public void Bounce()
        {
            if (isDeActivating) return;

            float duration = TileManager.Instance.beatInterval / 4;
            
            Vector3 scale = Vector3.one * 1.15f;

            selectButton.image.rectTransform.DOKill(true);
            selectButton.image.rectTransform.DOScale(scale, duration).SetLoops(2, LoopType.Yoyo);

            sideStageSlot.Bounce(bgmSelectIndex);
            mainStageSlot.Bounce(bgmSelectIndex);
        }
    }
}