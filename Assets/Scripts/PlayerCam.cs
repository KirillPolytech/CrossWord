using UnityEngine;
using Zenject;

public class PlayerCam : MonoBehaviour
{
    [Range(50, 500)][SerializeField] private int rayDistance = 100;
    [Range(1, 10)][SerializeField] private int scrollSens = 5;
    [Range(4, 10)][SerializeField] private int fovMin = 20;
    [Range(20, 40)][SerializeField] private int fovMax = 30;
    [Header("MovementSettings")]
    [Range(0f, 1f)][SerializeField] private float movementSpeed = 1f;
    [Range(25, 50)][SerializeField] private int limit = 50;

    private Camera _camera;
    private Ray _ray;
    private RaycastHit _hit;
    private InputHandler _inputHandler;
    private CharacterLogic _temp;
    private WindowsController _windowsController;

    [Inject]
    public void Construct(InputHandler inputHandler, WindowsController windowsController)
    {
        _inputHandler = inputHandler;
        _windowsController = windowsController;
        _camera = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        _inputHandler.UpdateCall += UpdateInput;
    }

    private void OnDisable()
    {
        _inputHandler.UpdateCall -= UpdateInput;
    }

    private void UpdateInput()
    {
        HandleScroll();
        HandleMovement();
        CastRay();
    }

    private void HandleScroll()
    {
        _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize - _inputHandler.MouseScrollValue * scrollSens, fovMin, fovMax);
    }

    private void HandleMovement()
    {
        transform.position += (Vector3.right * _inputHandler.HorizontalInputValue + Vector3.up * _inputHandler.VerticalInputValue) * movementSpeed;

        float x = Mathf.Clamp(transform.position.x, -limit ,limit);
        float y = Mathf.Clamp(transform.position.y, -limit ,limit);
        float z = Mathf.Clamp(transform.position.z, -limit ,limit);
        transform.position = new Vector3(x, y, z);
    }

    private void CastRay()
    {
        _ray = _camera.ScreenPointToRay(_inputHandler.MousePosition);

        Physics.Raycast(_ray, out _hit, rayDistance);

        if (_hit.collider is null || _hit.collider.gameObject.TryGetComponent(out CharacterLogic data) == false)
        {
            if (_temp == true)
                _temp.ChangeWordColor(ColorType.normal);
            
            _temp = null;
            return;
        }

        if (_temp is null)
        {
            _temp = data;
            data.ChangeWordColor(ColorType.highlighted);
        }
        else if (_temp == true && _temp != data)
        {
            _temp.ChangeWordColor(ColorType.normal);
            _temp = data;
            data.ChangeWordColor(ColorType.highlighted);
        }
        
        if (_inputHandler.LeftMouseButton == false)
            return;

        _windowsController.InputFieldWindow.Initialize(data);
        _windowsController.OpenWindow(_windowsController.InputFieldWindow);
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(_ray.origin, _ray.direction * rayDistance, Color.red);
    }
}