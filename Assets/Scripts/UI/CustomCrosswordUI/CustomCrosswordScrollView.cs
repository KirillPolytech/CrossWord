using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CustomCrosswordScrollView : MonoBehaviour
{
    [SerializeField] private RectTransform context;
    [SerializeField] private RectTransform customCrosswordButtonPrefab;
    [SerializeField] private Button addCrosswordButton;

    private List<RectTransform> _customCrosswords = new List<RectTransform>();
    private WindowsController _windowsController;

    [Inject]
    public void Construct(WindowsController windowsController)
    {
        _windowsController = windowsController;
    }

    private void Start()
    {
        addCrosswordButton.onClick.AddListener(() =>
        {
            _windowsController.OpenWindow("Creation");
        } );
    }

    public void CreateCustomCrosswordButton(Action action)
    {
        var temp = Instantiate(customCrosswordButtonPrefab, context);
        temp.GetComponentInChildren<TextMeshProUGUI>().text = $"Custom crossword {_customCrosswords.Count + 1}";
        temp.GetComponent<Button>().onClick.AddListener(() =>
        {
            action?.Invoke();
            _windowsController.OpenWindow("Creation");
        });
        
        _customCrosswords.Add(temp);
        
        //context.offsetMin = new Vector2(context.offsetMin.x - customCrosswordButtonPrefab.sizeDelta.x, context.offsetMin.y);
    }

    public void DeleteCustomCrosswordButton()
    {
        RectTransform temp = _customCrosswords.LastOrDefault();
        _customCrosswords.Remove(temp);
        
        if (temp)
            Destroy(temp.gameObject);
    }
}