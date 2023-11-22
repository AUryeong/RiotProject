using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum Direction
{
    Left,
    Up,
    Right,
    Down,
    Center
}

public class InputDetail : MonoBehaviour
{
    private const float AUTO_DRAG_POS = 130;
    private const float DRAG_MIN_POS = 20;

    [SerializeField] private EventTrigger inputEventTrigger;
    public Action<Direction> inputAction;

    public bool isOnlyLeftRight;
    public bool isOnlyDownUp;

    private Vector2 startDragPos;
    private Vector2 lastDragPos;
    private bool isDrag;

    private void Awake()
    {
        if (inputEventTrigger == null)
            inputEventTrigger = GetComponent<EventTrigger>();

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
            if (!isOnlyLeftRight && !isOnlyDownUp)
                inputAction?.Invoke(Direction.Center);

            return;
        }

        if (isOnlyLeftRight)
        {
            inputAction?.Invoke(distance.x > 0 ? Direction.Left : Direction.Right);
            return;
        }
        if (isOnlyDownUp)
        {
            inputAction?.Invoke(distance.y < 0 ? Direction.Up : Direction.Down);
            return;
        }

        if (distanceX > distanceY)
            inputAction?.Invoke(distance.x > 0 ? Direction.Left : Direction.Right);
        else
            inputAction?.Invoke(distance.y < 0 ? Direction.Up : Direction.Down);
    }
}