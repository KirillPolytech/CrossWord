using TMPro;
using UnityEngine;

public class CrosswordData : MonoBehaviour
{
    public Canvas canvas;
    public GameObject wordParentPrefab;
    public GameObject blockPrefab;
    public Color characterColor = Color.black;
    public TextMeshProUGUI description;
    [Range(0f, 1f)]public float distanceBetweenBlocks = 1f;
    [Range(2, 32)]public int crosswordLength = 10;
}
