using System;
using System.Collections.Generic;
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

    private readonly List<int> touches = new();


    private void Update()
    {
        CheckKeyInput();
        if (SaveManager.Instance.GameData.isButton)
            CheckTouch();
        else
            CheckSwipeTouch();
    }

    private void CheckTouch()
    {
#if UNITY_ANDROID
        if (Input.touchCount <= 0) return;
        var touch = Input.GetTouch(0);
        switch (touch.phase)
        {
            case TouchPhase.Began:
            case TouchPhase.Stationary:
            case TouchPhase.Moved:
                if (Input.touchCount > 1)
                {
                    var touch2 = Input.GetTouch(1);
                    if (touch2.phase == TouchPhase.Began)
                    {
                        inputAction?.Invoke(Direction.Down);
                        touches.Add(touch.fingerId);
                        touches.Add(touch2.fingerId);
                    }
                }
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                if (touches.Contains(touch.fingerId))
                {
                    touches.Remove(touch.fingerId);
                    return;
                }
                inputAction?.Invoke(touch.position.x <= Screen.width / 2 ? Direction.Left : Direction.Right);
                break;
        }
#endif
    }

    private void CheckSwipeTouch()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject(0)) return;
            
            isDrag = true;
            startDragPos = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            lastDragPos = Input.mousePosition;
            if (Vector2.Distance(startDragPos, lastDragPos) > AUTO_DRAG_POS)
                CheckInput();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            lastDragPos = Input.mousePosition;
            CheckInput();
        }
#elif UNITY_ANDROID
        if (Input.touchCount <= 0) return;
        var touch = Input.GetTouch(0);
        switch (touch.phase)
        {
            case TouchPhase.Began:
                if (EventSystem.current.IsPointerOverGameObject(0)) return;

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
#endif
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