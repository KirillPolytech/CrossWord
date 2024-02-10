using UnityEngine;

[RequireComponent (typeof(Light))]
public class Sun : MonoBehaviour
{
    [SerializeField] private Vector3 rotateDir;

    private Light _sun;
    private void Awake()
    {
        _sun = GetComponent<Light>();
    }

    private void FixedUpdate()
    {
        _sun.transform.Rotate(rotateDir);   
    }
}
