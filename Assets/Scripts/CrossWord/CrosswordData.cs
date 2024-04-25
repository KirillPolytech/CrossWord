using UnityEngine;
using UnityEngine.UI;
using Zenject;

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
    public string words;
    public string descriptions;

    [Inject]
    public void Construct(CrosswordPersistence crosswordPersistence)
    {
        words = crosswordPersistence.chosenCrossword;
        descriptions = crosswordPersistence.chosenDescription;
        
        slider.onValueChanged.AddListener( ChangeLength );
    }

    private void ChangeLength(float x)
    {
        crosswordLength = (int)x;
    }
}