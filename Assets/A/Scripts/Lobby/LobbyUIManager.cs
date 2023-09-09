using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Lobby
{
    public class LobbyUIManager : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private EventTrigger inputEventTrigger;

        [SerializeField] private Image runeIcon;
        [SerializeField] private TextMeshProUGUI runeText;

        [Header("Main Button")] [SerializeField]
        private Button startButton;

        [SerializeField] private Button bgmButton;
        [SerializeField] private Button settingButton;
        [SerializeField] private Button shopButton;

        [Header("BGM Select")] [SerializeField]
        private Image bgmMainSelect;

        [SerializeField] private TextMeshProUGUI bgmMainSelectText;

        [SerializeField] private Image bgmSideSelect;
        [SerializeField] private TextMeshProUGUI bgmSideSelectText;

        public const float UI_MOVE_DURATION = 1f;
        public const float UI_DRAG_MOVE_DURATION = 0.5f;
        public const float UI_SCALE_DURATION = 0.5f;

        private const float AUTO_DRAG_POS = 200;
        private const float DRAG_MIN_POS = 50;

        private bool isStarting;
        private bool isDrag;
        private Vector2 startDragPos;
        private Vector2 lastDragPos;

        [Header("Animation")] 
        private Sequence deActiveSequence;
        private Sequence activeSequence;

        private void Awake()
        {
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(StartButton);

            inputEventTrigger.AddListener(EventTriggerType.PointerDown, OnPointerDown);
            inputEventTrigger.AddListener(EventTriggerType.PointerUp, OnPointerUp);
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

        public void ActiveSetting()
        {
            isStarting = false;

            inputEventTrigger.gameObject.SetActive(true);
            
            int nowIndex = SaveManager.Instance.GameData.selectBgmIndex;

            bgmMainSelectText.text = TileManager.Instance.stageTileData.bgmDataList[nowIndex].bgmNickName;
            bgmSideSelectText.text = TileManager.Instance.stageTileData.bgmDataList[nowIndex].bgmNickName;

            if (activeSequence != null)
            {
                activeSequence.Restart();
                return;
            }
            activeSequence = DOTween.Sequence();

            activeSequence.SetAutoKill(false);
            activeSequence.OnStart(() =>
            {
                background.gameObject.SetActive(true);
                background.color = background.color.GetFade(0.3f);
                runeText.text = SaveManager.Instance.GameData.rune.ToString();
                bgmButton.image.rectTransform.localScale = Vector3.zero;
                settingButton.image.rectTransform.localScale = Vector3.zero;
                shopButton.image.rectTransform.localScale = Vector3.zero;
                runeIcon.rectTransform.anchoredPosition = new Vector2(100, runeIcon.rectTransform.anchoredPosition.y);
                startButton.image.rectTransform.anchoredPosition = new Vector2(startButton.image.rectTransform.anchoredPosition.x, -1200);
            });
            
            activeSequence.Join(bgmButton.image.rectTransform.DOScale(Vector3.zero, UI_SCALE_DURATION).SetEase(Ease.InBack));
            activeSequence.Join(runeIcon.rectTransform.DOAnchorPosX(-100, UI_MOVE_DURATION));
            activeSequence.Join(startButton.image.rectTransform.DOAnchorPosY(0, UI_MOVE_DURATION));
            activeSequence.Insert(UI_MOVE_DURATION, bgmButton.image.rectTransform.DOScale(Vector3.one, UI_SCALE_DURATION).SetEase(Ease.OutBack));
            activeSequence.Insert(UI_MOVE_DURATION + UI_SCALE_DURATION / 2, settingButton.image.rectTransform.DOScale(Vector3.one, UI_SCALE_DURATION).SetEase(Ease.OutBack));
            activeSequence.Insert(UI_MOVE_DURATION + UI_SCALE_DURATION, shopButton.image.rectTransform.DOScale(Vector3.one, UI_SCALE_DURATION).SetEase(Ease.OutBack));

            activeSequence.OnUpdate(() =>
            {
                if (!Input.GetMouseButtonDown(0)) return;
                activeSequence.Complete(true);
            });
        }

        public void DeActiveSetting()
        {
            inputEventTrigger.gameObject.SetActive(false);
            
            if (deActiveSequence != null)
            {
                deActiveSequence.Restart();
                return;
            }
            deActiveSequence = DOTween.Sequence();

            deActiveSequence.SetAutoKill(false);
            deActiveSequence.OnStart(() =>
            {
                background.color = background.color.GetFade(0.3f);
                bgmButton.image.rectTransform.localScale = Vector3.one;
                settingButton.image.rectTransform.localScale = Vector3.one;
                shopButton.image.rectTransform.localScale = Vector3.one;
                runeIcon.rectTransform.anchoredPosition = new Vector2(-100, runeIcon.rectTransform.anchoredPosition.y);
                startButton.image.rectTransform.anchoredPosition = Vector2.zero;
            });
            
            deActiveSequence.Join(bgmButton.image.rectTransform.DOScale(Vector3.zero, UI_SCALE_DURATION).SetEase(Ease.InBack));
            deActiveSequence.Join(runeIcon.rectTransform.DOAnchorPosX(100, UI_MOVE_DURATION));
            deActiveSequence.Insert(UI_SCALE_DURATION / 2, settingButton.image.rectTransform.DOScale(Vector3.zero, UI_SCALE_DURATION).SetEase(Ease.InBack));
            deActiveSequence.Insert(UI_SCALE_DURATION, shopButton.image.rectTransform.DOScale(Vector3.zero, UI_SCALE_DURATION).SetEase(Ease.InBack));
            deActiveSequence.Insert(UI_SCALE_DURATION * 2, startButton.image.rectTransform.DOAnchorPosY(-1200, UI_MOVE_DURATION));
            deActiveSequence.Join(background.DOFade(0, UI_MOVE_DURATION));

            deActiveSequence.OnUpdate(() =>
            {
                if (!Input.GetMouseButtonDown(0)) return;
                deActiveSequence.Complete(true);
            });
            
            deActiveSequence.OnComplete(() => gameObject.SetActive(false));
        }
    }
}