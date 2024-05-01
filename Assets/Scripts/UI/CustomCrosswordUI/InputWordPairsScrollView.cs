using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class InputWordPairsScrollView : MonoBehaviour
{
    [Range(0, 100)][SerializeField] private int minimumWords = 21;
    
    [SerializeField] private RectTransform context;
    [SerializeField] private RectTransform wordPairInputPrefab;
    [SerializeField] private Button startButton;
    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private TextMeshProUGUI wordsCounter;
    
    private int _crosswordIndex = -1;
    private List<CustomWord> _wordPairs = new List<CustomWord>();
    private bool _hasError;
    private CrosswordPersistence _crosswordPersistence;

    private const int Minsize = 100;
    private int _wordsCount;

    [Inject]
    public void Construct(CrosswordPersistence crosswordPersistence, WindowsController windowsController)
    {
        _crosswordPersistence = crosswordPersistence;
        
        startButton.onClick.AddListener(() =>
        {
            if (_crosswordIndex == -1)
            {
                Debug.LogWarning("_crosswordIndex == -1");
            }
            
            FinishCustomCrossword();

            if (_hasError == false)
            {
                SceneLoader.LoadScene(SceneLoader.MainScene);
            }
        });
    }
    
    public void Initialize(int index)
    {
        _crosswordIndex = index;

        wordsCounter.text = "";
        _wordsCount = 0;
        ClearScrollView();
        
        context.offsetMin = new Vector2(context.offsetMin.x,Minsize);
        context.offsetMax = new Vector2(context.offsetMax.x,0);
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
        
        UpdateWordCounterText(++_wordsCount);
    }

    public void RestoreInputFields(List<WordData> words)
    {
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
                    
            context.offsetMin = new Vector2(context.offsetMin.x, context.offsetMin.y - wordPairInputPrefab.sizeDelta.y);
            
            UpdateWordCounterText(++_wordsCount);
            
            // DEBUG: Debug.Log($"context.offsetMin {context.offsetMin}\n ");
        } 
    }

    public void DeleteLastWordInputField()
    {
        if (_wordPairs.Count <= minimumWords)
        {
            Debug.LogWarning("Empty");
            return;
        }

        CustomWord temp = _wordPairs.ElementAt(_wordPairs.Count - 1);
        _wordPairs.Remove(temp);
        Destroy(temp.Parent);
        
        UpdateWordCounterText(--_wordsCount);

        context.offsetMin = new Vector2(context.offsetMin.x, context.offsetMin.y + wordPairInputPrefab.sizeDelta.y);
    }

    private void DetectError()
    {
        errorText.text = "";
        _hasError = false;
        
        if (_wordPairs.Count < minimumWords)
        { 
            errorText.text += $"Error. The number of words does not exceed {minimumWords}.\n";
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

        CustomCrossword customCrossword = new CustomCrossword();

        for (int i = 0; i < _wordPairs.Count; i++)
        {
            customCrossword.words.Add(new WordData()
            {
                Word = _wordPairs[i].Word.text,
                WordDescription = _wordPairs[i].Description.text,
            });
        }
        
        _crosswordPersistence.SaveCrossword(customCrossword, _crosswordIndex);

        _crosswordPersistence.chosenCrossword = customCrossword.GetWords();
        _crosswordPersistence.chosenDescription = customCrossword.GetWordDescriptions();
    }

    private void ClearScrollView()
    {
        if (_wordPairs.Count <= 0)
            return;
        
        int length = _wordPairs.Count;
        for (int i = 0; i < length; i++)
        {
            Destroy(_wordPairs.ElementAt(i).Parent);
            Destroy(_wordPairs.ElementAt(i).Word.gameObject);
            Destroy(_wordPairs.ElementAt(i).Description.gameObject);
        }
        
        _wordPairs.Clear();
    }

    private void UpdateWordCounterText(int wordsCount)
    {
        wordsCounter.text = $"Words count: {wordsCount}";
    }
}