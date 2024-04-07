using System;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public event Action UpdateCall;

    public bool IsEnabled = false;

    public KeyCode HideInGameUIKey { get; private set; } = KeyCode.F1;
    public KeyCode InteractInGameMenu { get; private set; } = KeyCode.Escape;
    public float MouseScrollValue { get; private set; }
    public float HorizontalInputValue { get; private set; }
    public float VerticalInputValue { get; private set; }
    public Vector2 MousePosition { get; private set; }
    public bool LeftMouseButton { get; private set; }
    public bool RightMouseButton { get; private set; }
    private void Update()
    {
        MouseScrollValue = Input.mouseScrollDelta.y;
        HorizontalInputValue = Input.GetAxisRaw("Horizontal");
        VerticalInputValue = Input.GetAxisRaw("Vertical");
        MousePosition = Input.mousePosition;
        LeftMouseButton = Input.GetMouseButtonDown(0);
        RightMouseButton = Input.GetMouseButtonDown(1);
        
        if (IsEnabled == true)
            UpdateCall?.Invoke();
    }
}
