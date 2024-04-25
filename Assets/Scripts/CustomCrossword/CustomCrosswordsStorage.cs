using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;

public class CustomCrosswordsStorage
{
    private List<CustomCrossword> _crosswords;
    private InputWordPairsScrollView _inputWordPairsScroll;
    private CustomCrosswordScrollView _crosswordScroll;
    private CustomCrosswordsStorage _instance;
    private CrosswordPersistence _crosswordPersistence;

    [Inject]
    public void Construct(InputWordPairsScrollView inputWordPairsScroll, 
        CustomCrosswordScrollView crosswordScroll, 
        CrosswordPersistence crosswordPersistence, 
        CrosswordFilesStorage crosswordFilesStorage)
    {
        _inputWordPairsScroll = inputWordPairsScroll;
        _crosswordScroll = crosswordScroll;
        _crosswordPersistence = crosswordPersistence;

        Awake();
    }

    private void Awake()
    {
        _crosswords = _crosswordPersistence.LoadCrosswords();

        if (_crosswords == null)
        {
            _crosswords = new List<CustomCrossword>();
        }

        RestoreCrosswords();
    }

    public void CreateCrossword()
    {
        CustomCrossword customCrossword = new CustomCrossword();
        _crosswords.Add(customCrossword);
        
        _crosswordPersistence.SaveCrossword(customCrossword);

        _inputWordPairsScroll.Initialize(customCrossword);

        Action action = () =>
        {
            _crosswordPersistence.chosenCrossword = customCrossword.GetWords();
            _crosswordPersistence.chosenDescription = customCrossword.GetWordDescriptions();
            SceneLoader.LoadScene(SceneLoader.MainScene);
        };

        _crosswordScroll.CreateCustomCrosswordButton(action);
    }
    
    private void RestoreCrosswords()
    {
        foreach (var action in _crosswords.Select(c => (Action)(() =>
                 {
                     _crosswordPersistence.chosenCrossword = c.GetWords();
                     _crosswordPersistence.chosenDescription = c.GetWordDescriptions();
                     SceneLoader.LoadScene(SceneLoader.MainScene);
                 })))
        {
            _crosswordScroll.CreateCustomCrosswordButton(action);
        }
    }
}