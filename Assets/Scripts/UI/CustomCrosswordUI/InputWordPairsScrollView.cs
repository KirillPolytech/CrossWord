using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class InputWordPairsScrollView : MonoBehaviour
{
    [SerializeField] private RectTransform context;
    [SerializeField] private RectTransform wordPairInputPrefab;
    [SerializeField] private Button startButton;
    [SerializeField] private TextMeshProUGUI errorText;
    
    private CustomCrossword _customCrossword;
    private readonly List<CustomWord> _wordPairs = new List<CustomWord>();
    private bool _hasError;
    private CrosswordPersistence _crosswordPersistence;

    private const int Minsize = -200;

    [Inject]
    public void Construct(CrosswordPersistence crosswordPersistence, WindowsController windowsController)
    {
        _crosswordPersistence = crosswordPersistence;
        
        startButton.onClick.AddListener(() =>
        {
            FinishCustomCrossword();

            if (_hasError == false)
            {
                SceneLoader.LoadScene(SceneLoader.MainScene);
            }
        });
    }
    
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

    public void RestoreInputFields(List<WordData> words, CustomCrossword customCrossword)
    {
        Initialize(customCrossword);
        
        ClearScrollView();
        
        context.anchoredPosition = new Vector2(0,Minsize);
        
        for (int i = 0; i < words.Count; i++)
        {
            RectTransform temp = Instantiate(wordPairInputPrefab, context);

            CustomWord word = new CustomWord(
                temp.gameObject,
                temp.transform.GetChild(0).GetComponent<TMP_InputField>(),
                temp.transform.GetChild(1).GetComponent<TMP_InputField>());

            word.Word.text = words.ElementAt(i).Word;
            word.Description.text = words.ElementAt(i).WordDescription;
            
            _wordPairs.Add(word);
                    
            context.anchoredPosition = new Vector2(0, context.offsetMin.y - wordPairInputPrefab.sizeDelta.y);
        } 
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

    private void DetectError()
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
    }

    private void FinishCustomCrossword()
    {
        DetectError();
        if (_hasError == true)
        {
            return;
        }
        
        foreach (var wordPair in _wordPairs)
        {
            _customCrossword.AddWord(wordPair.Word.text, wordPair.Description.text);
        }
        
        _crosswordPersistence.SaveCrossword(_customCrossword);

        _crosswordPersistence.chosenCrossword = _customCrossword.GetWords();
        _crosswordPersistence.chosenDescription = _customCrossword.GetWordDescriptions();
    }

    private void ClearScrollView()
    {
        int length = _wordPairs.Count;
        for (int i = 0; i < length; i++)
        {
            Destroy(_wordPairs.ElementAt(i).Parent);
            Destroy(_wordPairs.ElementAt(i).Word.gameObject);
            Destroy(_wordPairs.ElementAt(i).Description.gameObject);
            _wordPairs.Remove(_wordPairs.ElementAt(i));
        }
    }
}