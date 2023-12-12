using System;
using UnityEngine;
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

    public Action<Direction> inputAction;

    public bool isOnlyLeftRight;
    public bool isOnlyDownUp;

    private Vector2 startDragPos;
    private Vector2 lastDragPos;
    private bool isDrag;

    private void Update()
    {
        CheckKeyInput();
        CheckTouch();
    }

    private void CheckTouch()
    {
        if (Input.touchCount <= 0) return;
            
        var touch = Input.GetTouch(0);
        switch (touch.phase)
        {
            case TouchPhase.Began:
                if (!EventSystem.current.IsPointerOverGameObject(0)) return;
                
                isDrag = true;
                startDragPos = touch.position;
                break;
            case TouchPhase.Moved:
                lastDragPos = touch.position;
                if (Vector2.Distance(startDragPos, lastDragPos) > AUTO_DRAG_POS)
                    CheckInput();
                break;
            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                lastDragPos = touch.position;
                CheckInput();
                break;
        }
    }

    private void CheckKeyInput()
    {
        if (!isOnlyLeftRight)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.S))
                inputAction?.Invoke(Direction.Down);

            if (Input.GetKeyDown(KeyCode.W))
                inputAction?.Invoke(Direction.Up);
        }

        if (!isOnlyDownUp)
        {
            if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.A))
                inputAction?.Invoke(Direction.Left);

            if (Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.D))
                inputAction?.Invoke(Direction.Right);
        }
    }

    private void CheckInput()
    {
        if (!isDrag) return;

        isDrag = false;
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