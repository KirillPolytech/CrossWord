using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class CustomCrosswordsStorage : IInitializable
{
    private List<CustomCrossword> _crosswords;
    private InputWordPairsScrollView _inputWordPairsScroll;
    private CustomCrosswordScrollView _crosswordScroll;
    private CustomCrosswordsStorage _instance;
    private CrosswordPersistence _crosswordPersistence;

    [Inject]
    public void Construct(
        InputWordPairsScrollView inputWordPairsScroll, 
        CustomCrosswordScrollView crosswordScroll, 
        CrosswordPersistence crosswordPersistence, 
        CrosswordFilesStorage crosswordFilesStorage)
    {
        _inputWordPairsScroll = inputWordPairsScroll;
        _crosswordScroll = crosswordScroll;
        _crosswordPersistence = crosswordPersistence;
    }
    
    public void Initialize()
    {
        _crosswords = _crosswordPersistence.LoadCrosswords();

        if (_crosswords == null)
        {
            _crosswords = new List<CustomCrossword>();
        }
        else
        {
            RestoreCrosswords();
            Debug.LogWarning("Crosswords restored.");
        }
    }

    public void Create()
    {
        if (_crosswords.Count > 0)
        {
            Debug.LogWarning("Crosswords limit exceeded.");
            return;
        }
        
        CustomCrossword customCrossword = new CustomCrossword();
        _crosswords.Add(customCrossword);
        
        _inputWordPairsScroll.Initialize(customCrossword);

        _crosswordScroll.CreateCustomCrosswordButton(null);
    }

    public void Delete()
    {
        CustomCrossword last = _crosswords.LastOrDefault();

        _crosswords.Remove(last);
        
        _crosswordScroll.DeleteCustomCrosswordButton();

        _crosswordPersistence.DeleteData();
    }
    
    private void RestoreCrosswords()
    {
        foreach (var t in _crosswords)
        {
            _crosswordScroll.CreateCustomCrosswordButton(() => _inputWordPairsScroll.RestoreInputFields(t.words, _crosswords.ElementAt(0)));
        }
    }
}