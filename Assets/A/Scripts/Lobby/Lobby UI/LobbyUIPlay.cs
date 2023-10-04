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

        [Space(10f)]
        [SerializeField] private Button settingButton;
        [SerializeField] private LobbyUISetting uiSetting;

        [Space(10f)]
        [SerializeField] private Image runeIcon;
        [SerializeField] private TextMeshProUGUI runeText;

        [Header("BGM Select")]
        [SerializeField] private RectTransform bgmParent;
        [SerializeField] private Image bgmMainSelect;

        [SerializeField] private TextMeshProUGUI bgmMainSelectText;

        [SerializeField] private Image bgmSideSelect;
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

            settingButton.onClick.RemoveAllListeners();
            settingButton.onClick.AddListener(uiSetting.Active);
        }

        public override void Active()
        {
            isStarting = false;

            inputEventTrigger.gameObject.SetActive(true);
            
            int nowIndex = SaveManager.Instance.GameData.selectBgmIndex;

            bgmMainSelectText.text = TileManager.Instance.stageTileData.bgmDataList[nowIndex].bgmNickName;
            bgmSideSelectText.text = TileManager.Instance.stageTileData.bgmDataList[nowIndex].bgmNickName;

            runeText.text = SaveManager.Instance.GameData.rune.ToString();

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
                settingButton.image.rectTransform.anchoredPosition = new Vector2(-100, settingButton.image.rectTransform.anchoredPosition.y);
                runeIcon.rectTransform.anchoredPosition = new Vector2(100, runeIcon.rectTransform.anchoredPosition.y);
                startButton.image.rectTransform.anchoredPosition = new Vector2(startButton.image.rectTransform.anchoredPosition.x, -1200);
                bgmParent.anchoredPosition = new Vector2(bgmParent.anchoredPosition.x, 1000);
            });
            
            activeSequence.Join(settingButton.image.rectTransform.DOAnchorPosX(100, UI_MOVE_DURATION));
            activeSequence.Join(runeIcon.rectTransform.DOAnchorPosX(-100, UI_MOVE_DURATION));
            activeSequence.Join(startButton.image.rectTransform.DOAnchorPosY(0, UI_MOVE_DURATION));
            activeSequence.Join(bgmParent.DOAnchorPosY(530, UI_MOVE_DURATION));

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
                settingButton.image.rectTransform.anchoredPosition = new Vector2(100, settingButton.image.rectTransform.anchoredPosition.y);
                runeIcon.rectTransform.anchoredPosition = new Vector2(-100, runeIcon.rectTransform.anchoredPosition.y);
                startButton.image.rectTransform.anchoredPosition = Vector2.zero;
                bgmParent.anchoredPosition = new Vector2(bgmParent.anchoredPosition.x, 530);
            });

            deActiveSequence.Join(settingButton.image.rectTransform.DOAnchorPosX(-100, UI_MOVE_DURATION));
            deActiveSequence.Join(runeIcon.rectTransform.DOAnchorPosX(100, UI_MOVE_DURATION));
            deActiveSequence.Join(startButton.image.rectTransform.DOAnchorPosY(-1200, UI_MOVE_DURATION));
            deActiveSequence.Join(bgmParent.DOAnchorPosY(1000, UI_MOVE_DURATION));

            deActiveSequence.OnComplete(() => gameObject.SetActive(false));
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
            bgmMainSelect.rectTransform.DOKill(true);
            bgmSideSelect.rectTransform.DOKill(true);

            int prevIndex = SaveManager.Instance.GameData.selectBgmIndex;
            int nowIndex = (prevIndex + 1) % TileManager.Instance.stageTileData.bgmDataList.Count;
            SaveManager.Instance.GameData.selectBgmIndex = nowIndex;

            if (prevIndex == nowIndex) return;

            bgmMainSelect.gameObject.SetActive(true);
            bgmMainSelect.rectTransform.anchoredPosition = new Vector2(-650, bgmMainSelect.rectTransform.anchoredPosition.y);
            bgmMainSelectText.text = TileManager.Instance.stageTileData.bgmDataList[nowIndex].bgmNickName;

            bgmSideSelect.rectTransform.DOAnchorPosX(650, UI_DRAG_MOVE_DURATION);
            bgmMainSelect.rectTransform.DOAnchorPosX(0, UI_DRAG_MOVE_DURATION).SetEase(Ease.OutBack).OnComplete(() =>
            {
                bgmSideSelect.rectTransform.anchoredPosition = new Vector2(0, bgmSideSelect.rectTransform.anchoredPosition.y);
                bgmSideSelectText.text = TileManager.Instance.stageTileData.bgmDataList[nowIndex].bgmNickName;

                TileManager.Instance.StageReset();

                bgmMainSelect.gameObject.SetActive(false);
            });
        }

        private void PrevBgm()
        {
            bgmMainSelect.rectTransform.DOKill(true);
            bgmSideSelect.rectTransform.DOKill(true);

            int prevIndex = SaveManager.Instance.GameData.selectBgmIndex;
            int nowIndex = prevIndex - 1 < 0 ? TileManager.Instance.stageTileData.bgmDataList.Count - 1 : prevIndex - 1;
            SaveManager.Instance.GameData.selectBgmIndex = nowIndex;

            if (prevIndex == nowIndex) return;

            bgmMainSelect.gameObject.SetActive(true);
            bgmMainSelect.rectTransform.anchoredPosition = new Vector2(650, bgmMainSelect.rectTransform.anchoredPosition.y);
            bgmMainSelectText.text = TileManager.Instance.stageTileData.bgmDataList[nowIndex].bgmNickName;

            bgmSideSelect.rectTransform.DOAnchorPosX(-650, UI_DRAG_MOVE_DURATION);
            bgmMainSelect.rectTransform.DOAnchorPosX(0, UI_DRAG_MOVE_DURATION).SetEase(Ease.OutBack).OnComplete(() =>
            {
                bgmSideSelect.rectTransform.anchoredPosition = new Vector2(0, bgmSideSelect.rectTransform.anchoredPosition.y);
                bgmSideSelectText.text = TileManager.Instance.stageTileData.bgmDataList[nowIndex].bgmNickName;

                TileManager.Instance.StageReset();

                bgmMainSelect.gameObject.SetActive(false);
            });
        }
    }
}