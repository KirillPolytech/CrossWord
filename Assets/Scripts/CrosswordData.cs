using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CrosswordData : MonoBehaviour
{
    public Canvas canvas;
    public Slider slider;
    public GameObject wordParentPrefab;
    public GameObject blockPrefab;
    public Color characterColor = Color.black;
    public TextMeshProUGUI description;
    [Range(0f, 1f)]public float distanceBetweenBlocks = 1f;
    [Range(7, 32)]public int crosswordLength = 7;
    public TextAsset words;
    public TextAsset descriptions;
    private void Awake()
    {
        slider.onValueChanged.AddListener( (x) => ChangeLength(x) );
    }

    private void ChangeLength(float x)
    {
        crosswordLength = (int)x;
    }
}
