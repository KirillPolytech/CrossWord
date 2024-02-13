using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    [Range(50, 500)][SerializeField] private int rayDistance = 100;
    [Range(1, 10)][SerializeField] private int scrollSens = 5;
    [Range(20, 40)][SerializeField] private int fovMin = 20;
    [Range(100, 300)][SerializeField] private int fovMax = 200;

    private float _mouseButton, _mouseScroll;   
    private Camera _camera;
    private RaycastHit _hit;
    private Ray _ray;
    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void Update()
    {
        HandleInput();
        HandleMovement();
        HandleScroll();
    }

    private void HandleInput()
    {
        _mouseButton = Input.GetMouseButton(1) ? 1 : 0;
        _mouseScroll = Input.mouseScrollDelta.y;
    }

    private void HandleScroll()
    {
        _camera.fieldOfView = Mathf.Clamp(_camera.fieldOfView - _mouseScroll * scrollSens, fovMin, fovMax);
    }

    private void HandleMovement()
    {
        if (_mouseButton == 0)
            return;

        _ray = _camera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(transform.position, _ray.direction, out _hit, rayDistance);

        _hit.point = new Vector3(_hit.point.x, _hit.point.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, _hit.point, Time.deltaTime );
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(_ray.origin, _ray.direction * rayDistance, Color.red);
    }
}
