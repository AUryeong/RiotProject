using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Lobby
{
    public class LobbyUIStage : LobbyUIActiveLink
    {
        [SerializeField] private EventTrigger inputEventTrigger;

        [SerializeField] private LobbyUIStageSlot mainStageSlot;
        [SerializeField] private LobbyUIStageSlot sideStageSlot;

        [SerializeField] private Button selectButton;
        [SerializeField] private Image borderBackground;

        [Space(10f)]

        [SerializeField] private Button exitButton;

        [Space(10f)]

        [SerializeField] private Image buttonOutline;
        [SerializeField] private Image buttonGradient;
        [SerializeField] private Image buttonIcon;

        private int stageSelectIndex;
        private int bgmSelectIndex;

        private const float UI_MOVE_DURATION = 0.75f;
        private const float UI_DRAG_MOVE_DURATION = 0.5f;

        private const float AUTO_DRAG_POS = 200;
        private const float DRAG_MIN_POS = 50;

        private bool isDeActivating;
        private bool isDrag;
        private Vector2 startDragPos;
        private Vector2 lastDragPos;

        [Header("Animation")]
        private Sequence deActiveSequence;
        private Sequence activeSequence;
        protected override void Awake()
        {
            inputEventTrigger.AddListener(EventTriggerType.PointerDown, OnPointerDown);
            inputEventTrigger.AddListener(EventTriggerType.PointerUp, OnPointerUp);

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

        public override void Active()
        {
            base.Active();
            isDeActivating = false;
            stageSelectIndex = SaveManager.Instance.GameData.selectStageIndex;
            inputEventTrigger.gameObject.SetActive(true);

            StageTileData stageTileData = TileManager.Instance.stageTileDataList[stageSelectIndex];
            mainStageSlot.ShowStage(stageTileData);
            sideStageSlot.ShowStage(stageTileData);

            buttonIcon.color = stageTileData.uiColor;
            buttonOutline.color = stageTileData.uiColor;
            buttonGradient.color = stageTileData.uiDarkColor;

            borderBackground.color = stageTileData.uiDarkColor.GetFade(0.35f);

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
                exitButton.image.rectTransform.anchoredPosition = new Vector2(-70, exitButton.image.rectTransform.anchoredPosition.y);
            });

            activeSequence.Join(sideStageSlot.RectTransform.DOScale(Vector3.one, UI_MOVE_DURATION).SetEase(Ease.OutBack));
            activeSequence.Join(selectButton.image.rectTransform.DOScale(Vector3.one, UI_MOVE_DURATION).SetEase(Ease.OutBack));
            activeSequence.Join(exitButton.image.rectTransform.DOAnchorPosX(70, UI_MOVE_DURATION));

            activeSequence.OnUpdate(() =>
            {
                if (!Input.GetMouseButtonDown(0)) return;
                activeSequence.Complete(true);
            });
        }

        public override void DeActive()
        {
            base.DeActive();
            inputEventTrigger.gameObject.SetActive(false);

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
            });

            deActiveSequence.Join(sideStageSlot.RectTransform.DOScale(Vector3.zero, UI_MOVE_DURATION / 2).SetEase(Ease.InBack));
            deActiveSequence.Join(selectButton.image.rectTransform.DOScale(Vector3.zero, UI_MOVE_DURATION / 2).SetEase(Ease.InBack));
            deActiveSequence.Join(exitButton.image.rectTransform.DOAnchorPosX(-70, UI_MOVE_DURATION / 2));

            deActiveSequence.OnComplete(() => gameObject.SetActive(false));
        }
        private void OnPointerDown(PointerEventData data)
        {
            isDrag = true;
            startDragPos = data.position;
        }

        private void Update()
        {
            if (!isDrag) return;

            lastDragPos = Input.mousePosition;
            if (Vector2.Distance(startDragPos, lastDragPos) > AUTO_DRAG_POS)
            {
                isDrag = false;
                CheckInput();
            }
        }

        private void OnPointerUp(PointerEventData data)
        {
            if (!isDrag) return;

            isDrag = false;
            CheckInput();
        }

        private void CheckInput()
        {
            float distance = startDragPos.x - lastDragPos.x;
            float distanceX = Mathf.Abs(distance);
            if (distanceX < DRAG_MIN_POS)
                return;

            if (distance > 0)
                PrevStage();
            else
                NextStage();
        }

        private void NextStage()
        {
            SoundManager.Instance.PlaySound("levelup_back", ESoundType.Sfx, 0.6f, 0.9f);

            mainStageSlot.RectTransform.DOKill(true);
            sideStageSlot.RectTransform.DOKill(true);

            stageSelectIndex = stageSelectIndex - 1 < 0 ? TileManager.Instance.stageTileDataList.Count - 1 : stageSelectIndex - 1;

            var stageTileData = TileManager.Instance.stageTileDataList[stageSelectIndex];

            mainStageSlot.gameObject.SetActive(true);
            mainStageSlot.RectTransform.anchoredPosition = new Vector2(-650, mainStageSlot.RectTransform.anchoredPosition.y);
            mainStageSlot.ShowStage(stageTileData);

            buttonIcon.DOKill(true);
            buttonIcon.DOColor(stageTileData.uiColor, UI_DRAG_MOVE_DURATION);

            buttonOutline.DOKill(true);
            buttonOutline.DOColor(stageTileData.uiColor, UI_DRAG_MOVE_DURATION);

            buttonGradient.DOKill(true);
            buttonGradient.DOColor(stageTileData.uiDarkColor, UI_DRAG_MOVE_DURATION);

            borderBackground.DOKill(true);
            borderBackground.DOColor(stageTileData.uiDarkColor.GetFade(0.35f), UI_DRAG_MOVE_DURATION);

            sideStageSlot.RectTransform.DOAnchorPosX(650, UI_DRAG_MOVE_DURATION).SetEase(Ease.OutBack);
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

            stageSelectIndex = (stageSelectIndex + 1) % TileManager.Instance.stageTileDataList.Count;

            var stageTileData = TileManager.Instance.stageTileDataList[stageSelectIndex];

            mainStageSlot.gameObject.SetActive(true);
            mainStageSlot.RectTransform.anchoredPosition = new Vector2(650, mainStageSlot.RectTransform.anchoredPosition.y);
            mainStageSlot.ShowStage(stageTileData);

            buttonIcon.DOKill(true);
            buttonIcon.DOColor(stageTileData.uiColor, UI_DRAG_MOVE_DURATION);

            buttonOutline.DOKill(true);
            buttonOutline.DOColor(stageTileData.uiColor, UI_DRAG_MOVE_DURATION);

            buttonGradient.DOKill(true);
            buttonGradient.DOColor(stageTileData.uiDarkColor, UI_DRAG_MOVE_DURATION);

            borderBackground.DOKill(true);
            borderBackground.DOColor(stageTileData.uiDarkColor.GetFade(0.35f), UI_DRAG_MOVE_DURATION);

            sideStageSlot.RectTransform.DOAnchorPosX(-650, UI_DRAG_MOVE_DURATION).SetEase(Ease.OutBack);
            mainStageSlot.RectTransform.DOAnchorPosX(0, UI_DRAG_MOVE_DURATION).SetEase(Ease.OutBack).OnComplete(() =>
            {
                sideStageSlot.RectTransform.anchoredPosition = new Vector2(0, sideStageSlot.RectTransform.anchoredPosition.y);
                sideStageSlot.ShowStage(stageTileData);
                mainStageSlot.gameObject.SetActive(false);

                bgmSelectIndex = Random.Range(0, stageTileData.bgmDataList.Count);
                TileManager.Instance.SetBgmData(stageTileData.bgmDataList[bgmSelectIndex]);
            });
        }
    }
}