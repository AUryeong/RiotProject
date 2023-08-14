using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : Singleton<InputManager>
{
    private const float AUTO_DRAG_POS = 300;
    
    [SerializeField] private EventTrigger inputEventTrigger;

    private Vector2 startDragPos;
    private Vector2 lastDragPos;
    private bool isDrag;

    protected override void OnCreated()
    {
        base.OnCreated();
        inputEventTrigger.AddListener(EventTriggerType.BeginDrag, OnBeginDrag);
        inputEventTrigger.AddListener(EventTriggerType.Drag, OnDrag);
        inputEventTrigger.AddListener(EventTriggerType.EndDrag, OnEndDrag);
    }

    private void OnBeginDrag(PointerEventData data)
    {
        isDrag = true;
        startDragPos = data.position;
    }

    private void OnDrag(PointerEventData data)
    {
        if (!isDrag) return;

        lastDragPos = data.position;
        if (Vector2.Distance(startDragPos, lastDragPos) > AUTO_DRAG_POS)
        {
            isDrag = false;
            CheckInput();
        }
    }

    private void OnEndDrag(PointerEventData data)
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

        if (distanceX > distanceY)
            Player.Instance.CheckInput(distance.x > 0 ? Direction.Left : Direction.Right);
        else
            Player.Instance.CheckInput(distance.y < 0 ? Direction.Up : Direction.Down);
    }
}