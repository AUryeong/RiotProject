using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGame
{
    [RequireComponent(typeof(Image))]
    public class UIGameOverJudgeSlot : MonoBehaviour
    {
        private Image image;

        private float defaultAlpha;

        private Sequence popupSequence;

        [HideIf("judgeChanger")]
        [SerializeField] private Image judgeIcon;
        
        [HideIf("judgeIcon")]
        [SerializeField] private UIButtonColorChanger judgeChanger;

        [SerializeField] private TextMeshProUGUI countText;

        private const float UI_MOVE_DURATION = 0.5f;

        private void Awake()
        {
            image = GetComponent<Image>();

            defaultAlpha = image.color.a;
        }

        public void Popup(int count, float delay = 0)
        {
            gameObject.SetActive(true);
            popupSequence?.Complete();

            if (judgeIcon != null)
            {
                judgeIcon.color = judgeIcon.color.GetAlpha(0);
                judgeIcon.rectTransform.anchoredPosition = new Vector2(-160, judgeIcon.rectTransform.anchoredPosition.y);
            }

            if (judgeChanger != null)
            {
                judgeChanger.Apply(TileManager.Instance.stageTileData.uiColor, TileManager.Instance.stageTileData.uiDarkColor);
                    
                judgeChanger.Apply(0);
                judgeChanger.RectTransform.anchoredPosition = new Vector2(-160, judgeChanger.RectTransform.anchoredPosition.y);

                judgeChanger.ApplyFade(UI_MOVE_DURATION, 1, delay + UI_MOVE_DURATION * 0.75f);
            }

            if (popupSequence != null)
            {
                popupSequence.Restart();
                return;
            }

            popupSequence = DOTween.Sequence();

            popupSequence.SetAutoKill(false);
            popupSequence.OnStart(() =>
            {
                countText.text = "0";

                image.color = image.color.GetAlpha(0);
                countText.color = countText.color.GetAlpha(0);
                countText.rectTransform.anchoredPosition = new Vector2(130, countText.rectTransform.anchoredPosition.y);
            });

            popupSequence.Insert(delay, image.DOFade(defaultAlpha, UI_MOVE_DURATION));
            if (judgeIcon != null)
            {
                popupSequence.Insert(delay + UI_MOVE_DURATION * 0.75f, judgeIcon.DOFade(1, UI_MOVE_DURATION));
                popupSequence.Insert(delay + UI_MOVE_DURATION * 0.75f, judgeIcon.rectTransform.DOAnchorPosX(-145, UI_MOVE_DURATION));
            }

            if (judgeChanger != null)
            {
                popupSequence.Insert(delay + UI_MOVE_DURATION * 0.75f, judgeChanger.RectTransform.DOAnchorPosX(-145, UI_MOVE_DURATION));
            }

            popupSequence.Insert(delay + UI_MOVE_DURATION * 0.75f, countText.DOFade(1, UI_MOVE_DURATION));
            popupSequence.Insert(delay + UI_MOVE_DURATION * 0.75f, countText.rectTransform.DOAnchorPosX(145, UI_MOVE_DURATION));

            popupSequence.Insert(delay + UI_MOVE_DURATION * 1.75f, countText.DOCounter(0, count, UI_MOVE_DURATION));
        }
    }
}