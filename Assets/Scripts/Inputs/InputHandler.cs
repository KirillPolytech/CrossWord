using System;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public event Action UpdateCall;
    public event Action FixedUpdateCall;
    public event Action AlwaysFixedUpdateCall;
    public event Action AlwaysUpdateCall;

    public bool IsEnabled { get; set; }
    
    public KeyCode CurrentKeyDown { get; private set; }

    public KeyCode GenerateCrosswordKey { get; set; } = KeyCode.Tab;
    public KeyCode HideInGameUIKey { get; private set; } = KeyCode.F1;
    public KeyCode InteractInGameMenu { get; private set; } = KeyCode.Escape;
    public bool Enter { get; private set; }
    public bool Space { get; private set; }
    public float MouseScrollValue { get; private set; }
    public float HorizontalInputValue { get; private set; }
    public float VerticalInputValue { get; private set; }
    public Vector2 MousePosition { get; private set; }
    public bool LeftMouseButton { get; private set; }
    public bool RightMouseButton { get; private set; }
    private void FixedUpdate()
    {
        if (IsEnabled == true)
        {
            FixedUpdateCall?.Invoke();
        }
        
        AlwaysFixedUpdateCall?.Invoke();
    }

    public void Update()
    {
        MouseScrollValue = Input.mouseScrollDelta.y;
        HorizontalInputValue = Input.GetAxisRaw("Horizontal");
        VerticalInputValue = Input.GetAxisRaw("Vertical");
        MousePosition = Input.mousePosition;
        LeftMouseButton = Input.GetMouseButtonDown(0);
        RightMouseButton = Input.GetMouseButtonDown(1);
        Space = Input.GetKeyDown(KeyCode.Space);
        Enter = Input.GetKeyDown(KeyCode.KeypadEnter);

        if (IsEnabled == true)
        {
            UpdateCall?.Invoke();
        }
        
        AlwaysUpdateCall?.Invoke();
    }

    private void OnGUI()
    {
        if (Event.current.isKey == true && Event.current.keyCode != KeyCode.None)
            CurrentKeyDown = Event.current.keyCode;
    }
}
