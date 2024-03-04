﻿using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class LobbyUIPlay : LobbyUIActiveLink
    {
        [SerializeField] private Button startButton;
        [SerializeField] private InputDetail inputDetail;

        [Header("Color Changer")]
        [SerializeField] private UIButtonColorChanger titleButtonChanger;
        [SerializeField] private UIButtonColorChanger playButtonChanger;
        [SerializeField] private UIButtonColorChanger lockButtonChanger;
        [SerializeField] private UIButtonColorChanger stageButtonChanger;

        [Header("Buttons")]
        [SerializeField] private Button shopButton;
        [SerializeField] private Button stageButton;

        [Header("Setting")]
        [SerializeField] private Button settingButton;
        [SerializeField] private LobbyUISetting uiSetting;

        [Header("Rune")]
        [SerializeField] private Image runeBase;
        [SerializeField] private TextMeshProUGUI runeText;

        [Header("BGM Select")]
        [SerializeField] private RectTransform bgmParent;

        [Space(10f)]
        [SerializeField] private Button bgmSelectLeft;
        [SerializeField] private Button bgmSelectRight;

        [Space(10f)]
        [SerializeField] private LobbyUISongSelect bgmMainSelect;

        [Space(10f)]
        [SerializeField] private LobbyUISongSelect bgmSideSelect;

        private const float UI_MOVE_DURATION = 0.75f;
        private const float UI_DRAG_MOVE_DURATION = 0.5f;

        private bool isDeActivating;
        private bool isActivating;

        [Header("Animation")]
        private Sequence deActiveSequence;
        private Sequence activeSequence;

        protected override void Awake()
        {
            inputDetail.inputAction = CheckInput;

            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(StartButton);

            shopButton.onClick.RemoveAllListeners();
            shopButton.onClick.AddListener(RankingButton);

            stageButton.onClick.RemoveAllListeners();
            stageButton.onClick.AddListener(StageButton);

            settingButton.onClick.RemoveAllListeners();
            settingButton.onClick.AddListener(uiSetting.Active);
            
            bgmSelectLeft.onClick.RemoveAllListeners();
            bgmSelectLeft.onClick.AddListener(PrevBgm);
            
            bgmSelectRight.onClick.RemoveAllListeners();
            bgmSelectRight.onClick.AddListener(NextBgm);
        }

        public override void Active()
        {
            base.Active();
            isDeActivating = false;
            isActivating = true;

            inputDetail.gameObject.SetActive(true);

            int nowIndex = SaveManager.Instance.GameData.selectBgmIndex;

            Color color = TileManager.Instance.stageTileData.uiColor;
            Color darkColor = TileManager.Instance.stageTileData.uiDarkColor;

            runeText.text = SaveManager.Instance.GameData.rune.ToString();
            runeText.fontSharedMaterial.SetColor("_OutlineColor", darkColor);

            titleButtonChanger.Apply(color, darkColor);
            playButtonChanger.Apply(color, darkColor);
            lockButtonChanger.Apply(color, darkColor);
            stageButtonChanger.Apply(color, darkColor);

            var stageData = SaveManager.Instance.GameData.GetSelectStageData();
            int lastScore = stageData.lastScore;
            int highScore = stageData.highScore;

            bgmSideSelect.gameObject.SetActive(true);
            bgmSideSelect.Show(TileManager.Instance.stageTileData.bgmDataList[nowIndex], color, lastScore, highScore);

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
                settingButton.image.rectTransform.anchoredPosition = new Vector2(-70, settingButton.image.rectTransform.anchoredPosition.y);
                runeBase.rectTransform.anchoredPosition = new Vector2(340, runeBase.rectTransform.anchoredPosition.y);
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
            activeSequence.Insert(UI_MOVE_DURATION / 2, startButton.image.rectTransform.DOScale(Vector3.one, UI_MOVE_DURATION).SetEase(Ease.OutBack));

            activeSequence.OnComplete(() => { isActivating = false; });

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
                settingButton.image.rectTransform.anchoredPosition = new Vector2(46, settingButton.image.rectTransform.anchoredPosition.y);
                runeBase.rectTransform.anchoredPosition = new Vector2(-160, runeBase.rectTransform.anchoredPosition.y);
                bgmParent.anchoredPosition = new Vector2(bgmParent.anchoredPosition.x, 350);
                startButton.image.rectTransform.localScale = Vector3.one;
                shopButton.image.rectTransform.localScale = Vector3.one;
                stageButton.image.rectTransform.localScale = Vector3.one;
            });

            deActiveSequence.Join(settingButton.image.rectTransform.DOAnchorPosX(-100, UI_MOVE_DURATION / 2));
            deActiveSequence.Join(runeBase.rectTransform.DOAnchorPosX(340, UI_MOVE_DURATION / 2));
            deActiveSequence.Join(bgmParent.DOAnchorPosY(1000, UI_MOVE_DURATION / 2));

            deActiveSequence.Join(startButton.image.rectTransform.DOScale(Vector3.zero, UI_MOVE_DURATION / 2).SetEase(Ease.InBack));
            deActiveSequence.Insert(UI_MOVE_DURATION / 8, stageButton.image.rectTransform.DOScale(Vector3.zero, UI_MOVE_DURATION / 2).SetEase(Ease.InBack));
            deActiveSequence.Insert(UI_MOVE_DURATION / 4, shopButton.image.rectTransform.DOScale(Vector3.zero, UI_MOVE_DURATION / 2).SetEase(Ease.InBack));

            deActiveSequence.OnComplete(() => gameObject.SetActive(false));
        }

        private void StageButton()
        {
            if (isDeActivating) return;

            isDeActivating = true;
            LobbyManager.Instance.uiManager.Select(LobbyType.Stage);

            SoundManager.Instance.PlaySound("Button", ESoundType.Sfx);
        }

        private void RankingButton()
        {
            //TODO PLAYGAMES
            //PlayGamesPlatform.Instance.ShowLeaderboardUI();

            SoundManager.Instance.PlaySound("Button", ESoundType.Sfx);
        }

        private void StartButton()
        {
            if (isDeActivating) return;

            isDeActivating = true;
            SoundManager.Instance.PlaySound("gamestart", ESoundType.Sfx, 1, 1.2f);
            GameManager.Instance.ActiveSceneLink(SceneLinkType.InGame);
        }

        private void CheckInput(Direction direction)
        {
            if (direction == Direction.Left)
                PrevBgm();
            else
                NextBgm();
        }

        private void NextBgm()
        {
            var selectableBgmIndexList = new List<int>();
            for (int i = 0; i < 3; i++)
            {
                if (SaveManager.Instance.GameData.GetStageData(SaveManager.Instance.GameData.selectStageIndex, i).isBuy)
                    selectableBgmIndexList.Add(i);
            }

            if (selectableBgmIndexList.Count <= 1) return;
            
            SoundManager.Instance.PlaySound("levelup_back", ESoundType.Sfx, 0.6f, 0.9f);

            bgmMainSelect.RectTransform.DOKill(true);
            bgmSideSelect.RectTransform.DOKill(true);
            bgmSelectRight.image.rectTransform.DOKill(true);

            int prevIndex = SaveManager.Instance.GameData.selectBgmIndex;
            int nowIndex = selectableBgmIndexList[prevIndex - 1 < 0 ? selectableBgmIndexList.Count - 1 : prevIndex - 1];
            SaveManager.Instance.GameData.selectBgmIndex = nowIndex;

            bgmMainSelect.gameObject.SetActive(true);
            bgmMainSelect.RectTransform.anchoredPosition = new Vector2(-GameManager.Instance.ScreenSize.x, bgmMainSelect.RectTransform.anchoredPosition.y);

            bgmSelectRight.image.rectTransform.DOPunchScale(Vector3.one * 0.6f, UI_DRAG_MOVE_DURATION);

            var stageData = SaveManager.Instance.GameData.GetSelectStageData();
            int lastScore = stageData.lastScore;
            int highScore = stageData.highScore;

            bgmMainSelect.Show(TileManager.Instance.stageTileData.bgmDataList[nowIndex], TileManager.Instance.stageTileData.uiColor, lastScore, highScore);

            bgmSideSelect.RectTransform.DOAnchorPosX(GameManager.Instance.ScreenSize.x, UI_DRAG_MOVE_DURATION);
            bgmMainSelect.RectTransform.DOAnchorPosX(0, UI_DRAG_MOVE_DURATION).SetEase(Ease.OutBack).OnComplete(() =>
            {
                bgmSideSelect.RectTransform.anchoredPosition = new Vector2(0, bgmSideSelect.RectTransform.anchoredPosition.y);
                bgmSideSelect.Show(TileManager.Instance.stageTileData.bgmDataList[nowIndex], TileManager.Instance.stageTileData.uiColor, lastScore, highScore);
                TileManager.Instance.StageReset();

                bgmMainSelect.gameObject.SetActive(false);
            });
        }

        private void PrevBgm()
        {
            var selectableBgmIndexList = new List<int>();
            for (int i = 0; i < 3; i++)
            {
                if (SaveManager.Instance.GameData.GetStageData(SaveManager.Instance.GameData.selectStageIndex, i).isBuy)
                    selectableBgmIndexList.Add(i);
            }

            if (selectableBgmIndexList.Count <= 1) return;
            
            SoundManager.Instance.PlaySound("levelup_back", ESoundType.Sfx, 0.6f, 0.9f);

            bgmMainSelect.RectTransform.DOKill(true);
            bgmSideSelect.RectTransform.DOKill(true);
            bgmSelectLeft.image.rectTransform.DOKill(true);

            int prevIndex = SaveManager.Instance.GameData.selectBgmIndex;
            int nowIndex = selectableBgmIndexList[prevIndex - 1 < 0 ? selectableBgmIndexList.Count - 1 : prevIndex - 1];
            SaveManager.Instance.GameData.selectBgmIndex = nowIndex;

            if (prevIndex == nowIndex) return;

            bgmMainSelect.gameObject.SetActive(true);
            bgmMainSelect.RectTransform.anchoredPosition = new Vector2(GameManager.Instance.ScreenSize.x, bgmMainSelect.RectTransform.anchoredPosition.y);

            bgmSelectLeft.image.rectTransform.DOPunchScale(Vector3.one * 0.6f, UI_DRAG_MOVE_DURATION);

            var stageData = SaveManager.Instance.GameData.GetSelectStageData();
            int lastScore = stageData.lastScore;
            int highScore = stageData.highScore;

            bgmMainSelect.Show(TileManager.Instance.stageTileData.bgmDataList[nowIndex], TileManager.Instance.stageTileData.uiColor, lastScore, highScore);

            bgmSideSelect.RectTransform.DOAnchorPosX(-GameManager.Instance.ScreenSize.x, UI_DRAG_MOVE_DURATION);
            bgmMainSelect.RectTransform.DOAnchorPosX(0, UI_DRAG_MOVE_DURATION).SetEase(Ease.OutBack).OnComplete(() =>
            {
                bgmSideSelect.RectTransform.anchoredPosition = new Vector2(0, bgmSideSelect.RectTransform.anchoredPosition.y);
                bgmSideSelect.Show(TileManager.Instance.stageTileData.bgmDataList[nowIndex], TileManager.Instance.stageTileData.uiColor, lastScore, highScore);
                TileManager.Instance.StageReset();

                bgmMainSelect.gameObject.SetActive(false);
            });
        }

        public void Bounce()
        {
            if (isDeActivating || isActivating) return;

            float duration = TileManager.Instance.BeatInterval / 4;

            Vector3 scale = Vector3.one * 1.05f;

            startButton.image.rectTransform.DOKill(true);
            startButton.image.rectTransform.DOScale(scale, duration).SetLoops(2, LoopType.Yoyo);

            stageButton.image.rectTransform.DOKill(true);
            stageButton.image.rectTransform.DOScale(scale, duration).SetLoops(2, LoopType.Yoyo);

            shopButton.image.rectTransform.DOKill(true);
            shopButton.image.rectTransform.DOScale(scale, duration).SetLoops(2, LoopType.Yoyo);

            titleButtonChanger.RectTransform.DOKill(true);
            titleButtonChanger.RectTransform.DOScale(scale * 0.9f, duration).SetLoops(2, LoopType.Yoyo);
        }
    }
}