using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class InputWordPairsScrollView : MonoBehaviour
{
    [SerializeField] private RectTransform context;
    [SerializeField] private RectTransform wordPairInputPrefab;

    private CustomCrossword _customCrossword;
    private List<CustomWord> _wordPairs = new List<CustomWord>();

    public void Initialize(CustomCrossword customCrossword)
    {
        _customCrossword = customCrossword;
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

    public void FinishCustomCrossword()
    {
        foreach (var wordPair in _wordPairs)
        {
            _customCrossword.AddWord(wordPair.Word.text, wordPair.Description.text);
        }
    }
}