using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomCrosswordScrollView : MonoBehaviour
{
    [SerializeField] private RectTransform context;
    [SerializeField] private RectTransform customCrosswordButtonPrefab;
    [SerializeField] private Button addCrosswordButton;

    private List<RectTransform> _customCrosswords = new List<RectTransform>();
    public void CreateCustomCrosswordButton(Action action)
    {
        var temp = Instantiate(customCrosswordButtonPrefab, context);
        temp.GetComponentInChildren<TextMeshProUGUI>().text = $"Custom crossword {_customCrosswords.Count + 1}";
        temp.GetComponent<Button>().onClick.AddListener(() => action?.Invoke());
        
        _customCrosswords.Add(temp);
        
        //context.offsetMin = new Vector2(context.offsetMin.x - customCrosswordButtonPrefab.sizeDelta.x, context.offsetMin.y);
    }                                                                                                                 
}