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
    [SerializeField] private TextMeshProUGUI errorText;

    public int CustomCrosswordsButtonsCount => _customCrosswordsButtons.Count;
    
    private List<RectTransform> _customCrosswordsButtons = new List<RectTransform>();
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
            errorText.text = _customCrosswordsButtons.Count < 5 ? "" : "Crosswords limit exceeded";
        } );
    }

    public void CreateCustomCrosswordButton(Action action)
    {
        var temp = Instantiate(customCrosswordButtonPrefab, context);
        temp.GetComponentInChildren<TextMeshProUGUI>().text = $"Custom crossword {_customCrosswordsButtons.Count + 1}";
        temp.GetComponent<Button>().onClick.AddListener(() =>
        {
            action?.Invoke();
            _windowsController.OpenWindow("Creation");
        });
        
        _customCrosswordsButtons.Add(temp);
        
        //context.offsetMin = new Vector2(context.offsetMin.x - customCrosswordButtonPrefab.sizeDelta.x, context.offsetMin.y);
    }

    public void DeleteCustomCrosswordButton()
    {
        RectTransform temp = _customCrosswordsButtons.LastOrDefault();
        _customCrosswordsButtons.Remove(temp);
        
        if (temp)
            Destroy(temp.gameObject);
    }
}