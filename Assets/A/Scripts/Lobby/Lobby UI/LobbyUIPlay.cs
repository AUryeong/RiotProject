using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Lobby
{
    public class LobbyUIPlay : LobbyUIActiveLink
    {
        [SerializeField] private Button startButton;
        [SerializeField] private EventTrigger inputEventTrigger;

        [Header("Title")]
        [SerializeField] private Image titleOutline;
        [SerializeField] private Image titleNeon;

        [Header("Buttons")]
        [SerializeField] private Button shopButton;
        [SerializeField] private Button stageButton;

        [Header("Play")]
        [SerializeField] private Image playHighLight;
        [SerializeField] private Image playGradient;
        [SerializeField] private Image playOutline;

        [Header("Shop")]
        [SerializeField] private Image lockOutline;
        [SerializeField] private Image lockIcon;
        [SerializeField] private Image lockGradient;

        [Header("Stage")]
        [SerializeField] private Image stageOutline;
        [SerializeField] private Image stageIcon;
        [SerializeField] private Image stageGradient;

        [Header("Setting")]
        [SerializeField] private Button settingButton;
        [SerializeField] private LobbyUISetting uiSetting;

        [Header("Rune")]
        [SerializeField] private Image runeBase;
        [SerializeField] private TextMeshProUGUI runeText;

        [Header("BGM Select")]
        [SerializeField] private RectTransform bgmParent;

        [Space(10f)]

        [SerializeField] private Image bgmSelectLeft;
        [SerializeField] private Image bgmSelectRight;

        [Space(10f)]

        [SerializeField] private RectTransform bgmMainSelect;
        [SerializeField] private Image bgmMainIcon;
        [SerializeField] private Image bgmMainIconGradient;
        [SerializeField] private TextMeshProUGUI bgmMainSelectText;

        [Space(10f)]

        [SerializeField] private RectTransform bgmSideSelect;
        [SerializeField] private Image bgmSideIcon;
        [SerializeField] private Image bgmSideIconGradient;
        [SerializeField] private TextMeshProUGUI bgmSideSelectText;

        private const float UI_MOVE_DURATION = 0.75f;
        private const float UI_DRAG_MOVE_DURATION = 0.5f;

        private const float AUTO_DRAG_POS = 200;
        private const float DRAG_MIN_POS = 50;

        private bool isStarting;
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

            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(StartButton);

            shopButton.onClick.RemoveAllListeners();
            shopButton.onClick.AddListener(LockButton);

            stageButton.onClick.RemoveAllListeners();
            stageButton.onClick.AddListener(StageButton);

            settingButton.onClick.RemoveAllListeners();
            settingButton.onClick.AddListener(uiSetting.Active);
        }

        public override void Active()
        {
            isStarting = false;

            inputEventTrigger.gameObject.SetActive(true);

            int nowIndex = SaveManager.Instance.GameData.selectBgmIndex;

            bgmSideSelectText.text = TileManager.Instance.stageTileData.bgmDataList[nowIndex].bgmNickName;

            bgmSideSelectText.ForceMeshUpdate(true);
            var charInfo = bgmSideSelectText.textInfo.characterInfo[0];
            bgmSideIconGradient.rectTransform.localPosition = (charInfo.topLeft + charInfo.bottomLeft) / 2 + new Vector3(-30, 0, 0);

            runeText.text = SaveManager.Instance.GameData.rune.ToString();

            Color color = TileManager.Instance.stageTileData.uiColor;
            Color darkColor = TileManager.Instance.stageTileData.uiDarkColor;

            runeText.fontSharedMaterial.SetColor("_OutlineColor", darkColor);

            titleOutline.color = color;
            titleNeon.color = color;

            bgmMainIcon.color = color;
            bgmSideIcon.color = color;

            playHighLight.color = color;
            playGradient.color = darkColor;
            playOutline.color = color;

            lockIcon.color = color;
            lockOutline.color = color;
            lockGradient.color = darkColor;

            stageIcon.color = color;
            stageOutline.color = color;
            stageGradient.color = darkColor;

            activeSequence?.Complete();
            deActiveSequence?.Complete();

            base.Active();
            if (activeSequence != null)
            {
                activeSequence.Restart();
                return;
            }
            activeSequence = DOTween.Sequence();

            activeSequence.SetAutoKill(false);
            activeSequence.OnStart(() =>
            {
                settingButton.image.rectTransform.anchoredPosition = new Vector2(-70, settingButton.image.rectTransform.anchoredPosition.y);
                runeBase.rectTransform.anchoredPosition = new Vector2(180, runeBase.rectTransform.anchoredPosition.y);
                bgmParent.anchoredPosition = new Vector2(bgmParent.anchoredPosition.x, 1000);
                startButton.image.rectTransform.localScale = Vector3.zero;
                shopButton.image.rectTransform.localScale = Vector3.zero;
                stageButton.image.rectTransform.localScale = Vector3.zero;
            });

            activeSequence.Join(settingButton.image.rectTransform.DOAnchorPosX(46f, UI_MOVE_DURATION));
            activeSequence.Join(runeBase.rectTransform.DOAnchorPosX(-160, UI_MOVE_DURATION));
            activeSequence.Join(bgmParent.DOAnchorPosY(350, UI_MOVE_DURATION));

            activeSequence.Join(shopButton.image.rectTransform.DOScale(Vector3.one, UI_MOVE_DURATION).SetEase(Ease.OutBack));
            activeSequence.Insert(UI_MOVE_DURATION / 4, stageButton.image.rectTransform.DOScale(Vector3.one, UI_MOVE_DURATION).SetEase(Ease.OutBack));
            activeSequence.Insert(UI_MOVE_DURATION/2, startButton.image.rectTransform.DOScale(Vector3.one, UI_MOVE_DURATION).SetEase(Ease.OutBack));

            activeSequence.OnUpdate(() =>
            {
                if (!Input.GetMouseButtonDown(0)) return;
                activeSequence.Complete(true);
            });
        }

        public override void DeActive()
        {
            inputEventTrigger.gameObject.SetActive(false);

            activeSequence?.Complete(true);
            deActiveSequence?.Complete(true);

            base.DeActive();
            if (deActiveSequence != null)
            {
                deActiveSequence.Restart();
                return;
            }
            deActiveSequence = DOTween.Sequence();

            deActiveSequence.SetAutoKill(false);
            deActiveSequence.OnStart(() =>
            {
                settingButton.image.rectTransform.anchoredPosition = new Vector2(46, settingButton.image.rectTransform.anchoredPosition.y);
                runeBase.rectTransform.anchoredPosition = new Vector2(-160, runeBase.rectTransform.anchoredPosition.y);
                bgmParent.anchoredPosition = new Vector2(bgmParent.anchoredPosition.x, 350);
                startButton.image.rectTransform.localScale = Vector3.one;
                shopButton.image.rectTransform.localScale = Vector3.one;
                stageButton.image.rectTransform.localScale = Vector3.one;
            });

            deActiveSequence.Join(settingButton.image.rectTransform.DOAnchorPosX(-100, UI_MOVE_DURATION / 2));
            deActiveSequence.Join(runeBase.rectTransform.DOAnchorPosX(100, UI_MOVE_DURATION / 2));
            deActiveSequence.Join(bgmParent.DOAnchorPosY(1000, UI_MOVE_DURATION / 2));

            deActiveSequence.Join(startButton.image.rectTransform.DOScale(Vector3.zero, UI_MOVE_DURATION / 2).SetEase(Ease.InBack));
            deActiveSequence.Insert(UI_MOVE_DURATION / 8, stageButton.image.rectTransform.DOScale(Vector3.zero, UI_MOVE_DURATION / 2).SetEase(Ease.InBack));
            deActiveSequence.Insert(UI_MOVE_DURATION / 4, shopButton.image.rectTransform.DOScale(Vector3.zero, UI_MOVE_DURATION / 2).SetEase(Ease.InBack));

            deActiveSequence.OnComplete(() => gameObject.SetActive(false));
        }

        private void StageButton()
        {
            if (isStarting) return;

            isStarting = true;
            SoundManager.Instance.PlaySound("Button", ESoundType.Sfx);
            LobbyManager.Instance.uiManager.Select(LobbyType.Stage);
        }

        private void LockButton()
        {
            shopButton.image.rectTransform.DOShakeAnchorPos(0.3f, 20, 50);
            SoundManager.Instance.PlaySound("Warning", ESoundType.Sfx);
        }

        private void StartButton()
        {
            if (isStarting) return;

            isStarting = true;
            GameManager.Instance.ActiveSceneLink(SceneLinkType.InGame);
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
                PrevBgm();
            else
                NextBgm();
        }

        private void NextBgm()
        {
            bgmMainSelect.DOKill(true);
            bgmSideSelect.DOKill(true);
            bgmSelectRight.rectTransform.DOKill(true);

            int prevIndex = SaveManager.Instance.GameData.selectBgmIndex;
            int nowIndex = prevIndex - 1 < 0 ? TileManager.Instance.stageTileData.bgmDataList.Count - 1 : prevIndex - 1;
            SaveManager.Instance.GameData.selectBgmIndex = nowIndex;

            if (prevIndex == nowIndex) return;

            bgmMainSelect.gameObject.SetActive(true);
            bgmMainSelect.anchoredPosition = new Vector2(-650, bgmMainSelect.anchoredPosition.y);

            bgmSelectRight.rectTransform.DOPunchScale(Vector3.one * 0.6f, UI_DRAG_MOVE_DURATION);

            bgmMainSelectText.text = TileManager.Instance.stageTileData.bgmDataList[nowIndex].bgmNickName;

            bgmMainSelectText.ForceMeshUpdate(true);
            var charInfo = bgmMainSelectText.textInfo.characterInfo[0];
            bgmMainIconGradient.rectTransform.localPosition = (charInfo.topLeft + charInfo.bottomLeft) / 2 + new Vector3(-30, 0, 0);

            bgmSideSelect.DOAnchorPosX(650, UI_DRAG_MOVE_DURATION);
            bgmMainSelect.DOAnchorPosX(0, UI_DRAG_MOVE_DURATION).SetEase(Ease.OutBack).OnComplete(() =>
            {
                bgmSideSelect.anchoredPosition = new Vector2(0, bgmSideSelect.anchoredPosition.y);
                bgmSideSelectText.text = TileManager.Instance.stageTileData.bgmDataList[nowIndex].bgmNickName;

                bgmSideSelectText.ForceMeshUpdate(true);
                var charInfo = bgmSideSelectText.textInfo.characterInfo[0];
                bgmSideIconGradient.rectTransform.localPosition = (charInfo.topLeft + charInfo.bottomLeft) / 2 + new Vector3(-30, 0, 0);

                TileManager.Instance.StageReset();

                bgmMainSelect.gameObject.SetActive(false);
            });
        }

        private void PrevBgm()
        {
            bgmMainSelect.DOKill(true);
            bgmSideSelect.DOKill(true);
            bgmSelectLeft.rectTransform.DOKill(true);

            int prevIndex = SaveManager.Instance.GameData.selectBgmIndex;
            int nowIndex = (prevIndex + 1) % TileManager.Instance.stageTileData.bgmDataList.Count;
            SaveManager.Instance.GameData.selectBgmIndex = nowIndex;

            if (prevIndex == nowIndex) return;

            bgmMainSelect.gameObject.SetActive(true);
            bgmMainSelect.anchoredPosition = new Vector2(650, bgmMainSelect.anchoredPosition.y);

            bgmSelectLeft.rectTransform.DOPunchScale(Vector3.one * 0.6f, UI_DRAG_MOVE_DURATION);

            bgmMainSelectText.text = TileManager.Instance.stageTileData.bgmDataList[nowIndex].bgmNickName;

            bgmMainSelectText.ForceMeshUpdate(true);
            var charInfo = bgmMainSelectText.textInfo.characterInfo[0];
            bgmMainIconGradient.rectTransform.localPosition = (charInfo.topLeft + charInfo.bottomLeft) / 2 + new Vector3(-30, 0, 0);

            bgmSideSelect.DOAnchorPosX(-650, UI_DRAG_MOVE_DURATION);
            bgmMainSelect.DOAnchorPosX(0, UI_DRAG_MOVE_DURATION).SetEase(Ease.OutBack).OnComplete(() =>
            {
                bgmSideSelect.anchoredPosition = new Vector2(0, bgmSideSelect.anchoredPosition.y);
                bgmSideSelectText.text = TileManager.Instance.stageTileData.bgmDataList[nowIndex].bgmNickName;

                bgmSideSelectText.ForceMeshUpdate(true);
                var charInfo = bgmSideSelectText.textInfo.characterInfo[0];
                bgmSideIconGradient.rectTransform.localPosition = (charInfo.topLeft + charInfo.bottomLeft) / 2 + new Vector3(-30, 0, 0);

                TileManager.Instance.StageReset();

                bgmMainSelect.gameObject.SetActive(false);
            });
        }
    }
}