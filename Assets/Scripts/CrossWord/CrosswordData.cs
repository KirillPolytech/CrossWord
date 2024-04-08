using UnityEngine;
using UnityEngine.UI;

public class CrosswordData : MonoBehaviour
{
    [Range(0f, 1f)] public float distanceBetweenBlocks = 1f;
    [Range(7, 10)] public int crosswordLength = 7;
    [Range(1, 10)] public int wordIntersection = 3;

    public Color characterColor = Color.black;
    public Canvas canvas;
    public Slider slider;
    public GameObject wordParentPrefab;
    public GameObject blockPrefab;
    public TextAsset words;
    public TextAsset descriptions;
    private void Awake()
    {
        GamePreference gamePreference = FindAnyObjectByType<GamePreference>();

        words = gamePreference.ChoosenCrossword;
        descriptions = gamePreference.ChoosenDescription;
        
        slider.onValueChanged.AddListener( ChangeLength );
    }

    private void ChangeLength(float x)
    {
        crosswordLength = (int)x;
    }
}
