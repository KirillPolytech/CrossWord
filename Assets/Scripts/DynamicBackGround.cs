using UnityEngine;

public class DynamicBackGround : MonoBehaviour
{
    [Range(0, 1f)][SerializeField] private float speed;
    
    private Camera _camera;
    private Color _initialColor, _targetColor;
    private float _value = 0;
    private const float Approximation = 0.9999f;
    private const float ColorIntensityLimit = 0.25f;

    private void Awake()
    {
        _camera = GetComponent<Camera>();

        _initialColor = _camera.backgroundColor;
    }

    private void Update()
    {
        SmoothChangeColor();
    }

    private void SmoothChangeColor()
    {
        _camera.backgroundColor = new Color(
            Mathf.Lerp(_initialColor.r, _targetColor.r, _value), 
            Mathf.Lerp(_initialColor.g, _targetColor.g, _value), 
            Mathf.Lerp(_initialColor.b, _targetColor.b, _value)
            );
        
        _value = Mathf.Clamp( _value + speed * Time.deltaTime, 0 , 1);

        if (_value < Approximation) 
            return;
        
        _camera.backgroundColor = _targetColor;
        SetNewTargetColor();
    }

    private void SetNewTargetColor()
    {
        _initialColor = _camera.backgroundColor;
        _targetColor = new Color(Random.Range(0f, ColorIntensityLimit), Random.Range(0f, ColorIntensityLimit), Random.Range(0f, ColorIntensityLimit), 1);

        _value = 0;
    }
}