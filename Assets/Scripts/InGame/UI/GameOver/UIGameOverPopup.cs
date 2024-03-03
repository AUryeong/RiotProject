using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGame
{
    public class UIGameOverPopup : MonoBehaviour
    {
        private float toLobbyDuration = 0;

        [SerializeField] private Image background;
        [SerializeField] private Image blackBackground;

        [Space(10f)]
        [SerializeField] private Image rankBackground;
        [SerializeField] private TextMeshProUGUI rankText;

        [Space(10f)]
        [SerializeField] private RectTransform resultBase;
        [SerializeField] private UIButtonColorChanger gameOverChanger;
        [SerializeField] private UIButtonColorChanger fullComboChanger;
        [Space(10f)]
        [SerializeField] private UIButtonColorChanger gameClearChanger;

        [Space(10f)]
        [SerializeField] private RectTransform runeBase;
        [SerializeField] private TextMeshProUGUI runeText;

        [Space(10f)]
        [SerializeField] private UIGameOverJudgeSlot comboJudgeSlot;
        [SerializeField] private UIGameOverJudgeSlot prefectJudgeSlot;
        [SerializeField] private UIGameOverJudgeSlot greatJudgeSlot;
        [SerializeField] private UIGameOverJudgeSlot goodJudgeSlot;
        [SerializeField] private UIGameOverJudgeSlot missJudgeSlot;

        private const float UI_MOVE_DURATION = 1f;

        public void Popup()
        {
            gameObject.SetActive(true);
            runeText.text = string.Empty;

            background.color = background.color.GetAlpha(0);
            blackBackground.color = blackBackground.color.GetAlpha(0);

            var lightColor = TileManager.Instance.stageTileData.uiColor;
            var darkColor = TileManager.Instance.stageTileData.uiDarkColor;

            resultBase.anchoredPosition = new Vector2(resultBase.anchoredPosition.x, -228f);

            runeBase.localScale = Vector3.zero;

            rankBackground.color = rankBackground.color.GetAlpha(0);
            rankText.color = rankText.color.GetAlpha(0);
            rankText.rectTransform.localScale = Vector3.one * 10;
            rankText.rectTransform.localRotation = Quaternion.Euler(0, 0, 0);
            rankText.fontMaterial.SetColor("_OutlineColor", darkColor);

            int runeCount = InGameManager.Instance.Rune;
            int maxRuneCount = TileManager.Instance.bgmData.DefaultBeatCount * Item_Rune.PERFECT_RUNE_COUNT;

            string rank = "F";
            if (runeCount >= maxRuneCount * 6 / 6f)
                rank = "S";
            else if (runeCount >= maxRuneCount * 5 / 6f)
                rank = "A";
            else if (runeCount >= maxRuneCount * 4 / 6f)
                rank = "B";
            else if (runeCount >= maxRuneCount * 3 / 6f)
                rank = "C";
            else if (runeCount >= maxRuneCount * 2 / 6f)
                rank = "D";
            else if (runeCount >= maxRuneCount * 1 / 6f)
                rank = "E";

            rankText.text = rank;

            fullComboChanger.gameObject.SetActive(false);

            comboJudgeSlot.gameObject.SetActive(false);
            prefectJudgeSlot.gameObject.SetActive(false);
            greatJudgeSlot.gameObject.SetActive(false);
            goodJudgeSlot.gameObject.SetActive(false);
            missJudgeSlot.gameObject.SetActive(false);

            blackBackground.DOFade(0.35f, UI_MOVE_DURATION);
            background.DOFade(1, UI_MOVE_DURATION).SetDelay(UI_MOVE_DURATION);

            resultBase.DOAnchorPosY(-162.5f, UI_MOVE_DURATION).SetDelay(UI_MOVE_DURATION);

            if (Player.Instance.IsAlive)
            {
                gameClearChanger.gameObject.SetActive(true);
                gameOverChanger.gameObject.SetActive(false);

                gameClearChanger.Apply(lightColor, darkColor);
                gameClearChanger.Apply(0);

                gameClearChanger.ApplyFade(UI_MOVE_DURATION, 1, UI_MOVE_DURATION);

                if (InGameManager.Instance.GetBeatHit(BeatHitType.Miss) <= 0)
                {
                    fullComboChanger.gameObject.SetActive(true);

                    fullComboChanger.Apply(lightColor, darkColor);
                    fullComboChanger.Apply(0);
                    fullComboChanger.RectTransform.anchoredPosition = new Vector2(-15f, fullComboChanger.RectTransform.anchoredPosition.y);

                    fullComboChanger.ApplyFade(UI_MOVE_DURATION, 1, UI_MOVE_DURATION * 1.25f);
                    fullComboChanger.RectTransform.DOAnchorPosX(0, UI_MOVE_DURATION).SetDelay(UI_MOVE_DURATION * 1.25f).OnComplete(() =>
                    {
                        SoundManager.Instance.PlaySound("fullcombo", ESoundType.Sfx);
                    });
                }
            }
            else
            {
                gameOverChanger.gameObject.SetActive(true);
                gameClearChanger.gameObject.SetActive(false);

                gameOverChanger.Apply(lightColor, darkColor);
                gameOverChanger.Apply(0);

                gameOverChanger.ApplyFade(UI_MOVE_DURATION, 1, UI_MOVE_DURATION);
            }

            comboJudgeSlot.Popup(InGameManager.Instance.BeatHitCount, UI_MOVE_DURATION * 1.75f);
            prefectJudgeSlot.Popup(InGameManager.Instance.GetBeatHit(BeatHitType.Perfect), UI_MOVE_DURATION * 2f);
            greatJudgeSlot.Popup(InGameManager.Instance.GetBeatHit(BeatHitType.Great), UI_MOVE_DURATION * 2.25f);
            goodJudgeSlot.Popup(InGameManager.Instance.GetBeatHit(BeatHitType.Good), UI_MOVE_DURATION * 2.5f);
            missJudgeSlot.Popup(InGameManager.Instance.GetBeatHit(BeatHitType.Miss), UI_MOVE_DURATION * 2.75f);

            runeBase.DOScale(1, UI_MOVE_DURATION).SetEase(Ease.OutBack).SetDelay(UI_MOVE_DURATION * 3f);
            runeText.DOCounter(0, runeCount, UI_MOVE_DURATION * 2).SetDelay(UI_MOVE_DURATION * 3f);

            rankBackground.DOFade(0.1f, UI_MOVE_DURATION / 4f).SetDelay(UI_MOVE_DURATION * 5.5f);
            rankText.rectTransform.DOScale(1, UI_MOVE_DURATION / 4).SetDelay(UI_MOVE_DURATION * 5.5f).OnStart(() =>
            {
                SoundManager.Instance.PlaySound("Drum_snare", ESoundType.Sfx, 1, 0.7f + ((float)runeCount / maxRuneCount * 0.6f));
            });
            rankText.rectTransform.DORotate(new Vector3(0, 0, Random.Range(-35f, 35f)), UI_MOVE_DURATION / 4).SetDelay(UI_MOVE_DURATION * 5.5f);
            rankText.DOFade(1, UI_MOVE_DURATION / 4).SetDelay(UI_MOVE_DURATION * 5.5f);

            toLobbyDuration = 8;
        }

        private void Update()
        {
            if (toLobbyDuration <= 0) return;

            toLobbyDuration -= Time.deltaTime;

            if (toLobbyDuration >= 2f && Input.GetMouseButtonDown(0))
            {
                toLobbyDuration = 1;
                DOTween.CompleteAll();
            }

            if (toLobbyDuration > 0) return;
            GameManager.Instance.ActiveSceneLink(SceneLinkType.Lobby);
        }
    }
}