using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGame
{
    public class UIGameOverPopup : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private Image gameOverImage;

        [Space(10f)]
        [SerializeField] private RectTransform runeBase;
        [SerializeField] private TextMeshProUGUI runeText;
        
        [Space(10f)]
        [SerializeField] private UIGameOverJudgeSlot prefectJudgeSlot;
        [SerializeField] private UIGameOverJudgeSlot greatJudgeSlot;
        [SerializeField] private UIGameOverJudgeSlot goodJudgeSlot;
        [SerializeField] private UIGameOverJudgeSlot missJudgeSlot;

        private Sequence popupSequence;

        public void Popup()
        {
            gameObject.SetActive(true);
            
            popupSequence?.Complete();
            if (popupSequence != null)
            {
                popupSequence.Restart();
                return;
            }

            popupSequence = DOTween.Sequence();

            popupSequence.SetAutoKill(false);
            popupSequence.OnStart(() =>
            {
                
            });

        }
    }
}
