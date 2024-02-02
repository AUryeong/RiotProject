using DG.Tweening;
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
        private float defaultPosY;

        private Sequence popupSequence;

        [SerializeField] private Image judgeIcon;
        [SerializeField] private TextMeshProUGUI countText;

        private const float UI_MOVE_DURATION = 1;

        private void Awake()
        {
            image = GetComponent<Image>();
            
            defaultAlpha = image.color.a;
            defaultPosY = image.rectTransform.anchoredPosition.y;
        }
        
        public void Popup(int count, float delay= 0)
        {
            gameObject.SetActive(true);
            countText.text = count.ToString();
            
            popupSequence?.Complete();
            popupSequence.SetDelay(delay);
            
            if (popupSequence != null)
            {
                popupSequence.Restart();
                return;
            }

            popupSequence = DOTween.Sequence();

            popupSequence.SetAutoKill(false);
            popupSequence.OnStart(() =>
            {
                image.color = image.color.GetAlpha(0);
                judgeIcon.color = judgeIcon.color.GetAlpha(0);
                countText.color = countText.color.GetAlpha(0);
                image.rectTransform.anchoredPosition = new Vector2(image.rectTransform.anchoredPosition.x, defaultPosY - 15f);
            });

            popupSequence.Join(image.DOFade(defaultAlpha, UI_MOVE_DURATION));
            popupSequence.Insert(UI_MOVE_DURATION * 0.75f, judgeIcon.DOFade(defaultAlpha, UI_MOVE_DURATION));
            popupSequence.Insert(UI_MOVE_DURATION * 0.75f, countText.DOFade(1, UI_MOVE_DURATION));
            popupSequence.Insert(UI_MOVE_DURATION * 0.75f, image.rectTransform.DOAnchorPosY(defaultPosY, UI_MOVE_DURATION));
        }
    }
}