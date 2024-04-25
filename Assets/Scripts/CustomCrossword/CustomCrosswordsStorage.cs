using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class CustomCrosswordsStorage : MonoBehaviour
{
    private static List<CustomCrossword> _crosswords;
    private InputWordPairsScrollView _inputWordPairsScroll;
    private CustomCrosswordScrollView _crosswordScroll;
    private CrosswordFilesStorage _crosswordFilesStorage;
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
        _crosswordFilesStorage = crosswordFilesStorage;
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

    private void RestoreCrosswords()
    {
        foreach (var action in _crosswords.Select(c => (Action)(() =>
                 {
                     _crosswordFilesStorage.chosenCrossword = c.GetWords();
                     _crosswordFilesStorage.chosenDescription = c.GetWordDescriptions();
                     SceneLoader.LoadScene(SceneLoader.MainScene);
                 })))
        {
            _crosswordScroll.CreateCustomCrosswordButton(action);
        }
    }

    public void CreateCrossword()
    {
        CustomCrossword customCrossword = new CustomCrossword();
        _crosswords.Add(customCrossword);

        _inputWordPairsScroll.Initialize(customCrossword);

        Action action = () =>
        {
            _crosswordFilesStorage.chosenCrossword = customCrossword.GetWords();
            _crosswordFilesStorage.chosenDescription = customCrossword.GetWordDescriptions();
            SceneLoader.LoadScene(SceneLoader.MainScene);
        };

        _crosswordScroll.CreateCustomCrosswordButton(action);
    }
}