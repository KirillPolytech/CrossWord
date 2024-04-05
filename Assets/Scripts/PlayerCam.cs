using System;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    [Range(50, 500)][SerializeField] private int rayDistance = 100;
    [Range(1, 10)][SerializeField] private int scrollSens = 5;
    [Range(4, 10)][SerializeField] private int fovMin = 20;
    [Range(20, 40)][SerializeField] private int fovMax = 30;
    [Header("MovementSettings")]
    [Range(0f, 1f)][SerializeField] private float movementSpeed = 1f;

    private float _mouseScroll;
    private Camera _camera;
    private Ray _ray;
    private RaycastHit _hit;
    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void Update()
    {
        HandleInput();
        HandleMovement();
        HandleScroll();
        CastRay();
    }

    private void HandleInput()
    {
        _mouseScroll = Input.mouseScrollDelta.y;
    }

    private void HandleScroll()
    {
        _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize - _mouseScroll * scrollSens, fovMin, fovMax);
    }

    private void HandleMovement()
    {
        transform.position += ( Vector3.right * Input.GetAxis("Horizontal") + Vector3.up * Input.GetAxis("Vertical") ) * movementSpeed;
    }

    private void CastRay()
    {
        if (Input.GetButton("Fire1") == false)
            return;

        _ray = _camera.ScreenPointToRay(Input.mousePosition);

        Physics.Raycast(_ray, out _hit, rayDistance);

        if (_hit.collider == null || _hit.collider.gameObject.TryGetComponent(out CharacterData data) == false)
            return;

        data.OpenPanel();
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(_ray.origin, _ray.direction * rayDistance, Color.red);
    }
}