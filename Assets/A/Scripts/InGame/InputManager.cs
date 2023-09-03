using UnityEngine;
using UnityEngine.EventSystems;

namespace InGame
{
    public class InputManager : Singleton<InputManager>
    {
        private const float AUTO_DRAG_POS = 250;
        private const float DRAG_MIN_POS = 20;
    
        [SerializeField] private EventTrigger inputEventTrigger;

        private Vector2 startDragPos;
        private Vector2 lastDragPos;
        private bool isDrag;

        protected override void OnCreated()
        {
            base.OnCreated();
            inputEventTrigger.AddListener(EventTriggerType.PointerDown, OnPointerDown);
            inputEventTrigger.AddListener(EventTriggerType.PointerUp, OnPointerUp);
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
            Vector2 distance = startDragPos - lastDragPos;
            float distanceX = Mathf.Abs(distance.x);
            float distanceY = Mathf.Abs(distance.y);
            if (distanceX < DRAG_MIN_POS && distanceY < DRAG_MIN_POS)
            {
                Player.Instance.CheckInput(Direction.Down);
                return;
            }

            if (distanceX > distanceY)
                Player.Instance.CheckInput(distance.x > 0 ? Direction.Left : Direction.Right);
            else
                Player.Instance.CheckInput(distance.y < 0 ? Direction.Up : Direction.Down);
        }
    }
}