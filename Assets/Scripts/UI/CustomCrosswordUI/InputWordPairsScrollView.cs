using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputWordPairsScrollView : MonoBehaviour
{
    [SerializeField] private RectTransform context;
    [SerializeField] private RectTransform wordPairInputPrefab;
    [SerializeField] private Button finishButton;
    [SerializeField] private TextMeshProUGUI errorText;

    [Header("Canvases")] 
    [SerializeField] private Canvas CreateCrosswordCanvas; 
    [SerializeField] private Canvas CrosswordsCanvas;

    private CustomCrossword _customCrossword;
    private readonly List<CustomWord> _wordPairs = new List<CustomWord>();
    private bool _hasError;
    public void Initialize(CustomCrossword customCrossword)
    {
        _customCrossword = customCrossword;
        
        finishButton.onClick.AddListener(() =>
        {
            FinishCustomCrossword();

            if (_hasError == false)
            {
                CreateCrosswordCanvas.enabled = false;
                CrosswordsCanvas.enabled = true;
            }
        });
    }

    public void AddWordInputField()
    {
        RectTransform temp = Instantiate(wordPairInputPrefab, context);
        _wordPairs.Add(
            new CustomWord(
                temp.gameObject,
                temp.transform.GetChild(0).GetComponent<TMP_InputField>(),
                temp.transform.GetChild(1).GetComponent<TMP_InputField>()
            ));

        context.offsetMin = new Vector2(context.offsetMin.x, context.offsetMin.y - wordPairInputPrefab.sizeDelta.y);
    }

    public void DeleteLastWordInputField()
    {
        if (_wordPairs.Count <= 0)
        {
            Debug.LogWarning("Empty");
            return;
        }

        CustomWord temp = _wordPairs.ElementAt(_wordPairs.Count - 1);
        _wordPairs.Remove(temp);
        Destroy(temp.Parent);

        context.offsetMin = new Vector2(context.offsetMin.x, context.offsetMin.y + wordPairInputPrefab.sizeDelta.y);
    }

    private void FinishCustomCrossword()
    {
        errorText.text = "";
        _hasError = false;
        
        if (_wordPairs.Count < 14)
        { 
            errorText.text += "Error. The number of words does not exceed 14.\n";
            _hasError = true;
        }
        
        if (_wordPairs.Where(x => x.Description.text == string.Empty).ToList().Count != 0)
        {
            errorText.text += "Error. The word descriptions are not fully filled in.\n";
            _hasError = true;
        }
        
        if (_wordPairs.Where(x => x.Word.text == string.Empty).ToList().Count != 0)
        {
            errorText.text += "Error. The words are not fully filled in.\n";
            _hasError = true;
        }

        if (_hasError == true)
        {
            return;
        }
        
        foreach (var wordPair in _wordPairs)
        {
            _customCrossword.AddWord(wordPair.Word.text, wordPair.Description.text);
        }
    }
}