using System.Collections.Generic;
using CrosswordWindows;
using TMPro;
using UnityEngine;
using Zenject;

public class CrosswordUI : Window
{
    [SerializeField] private TextMeshProUGUI tutorial;
    [SerializeField] private TextMeshProUGUI wordDescriptions;
    [SerializeField] private TextMeshProUGUI errorText;

    private CharacterLogic _characterLogic;
    private bool _isInputMenuOpened;
    
    private InputHandler _inputHandler;
    private CrossWord _crossWord;
    private InGameMenu _inGameMenu;
    private WindowsController _windowsController;
    private GamePreference _gamePreference;

    [Inject]
    public void Construct(
        WindowsController windowsController, 
        InputHandler inputHandler, 
        CrossWord crossWord, 
        InGameMenu inGameMenu, 
        GamePreference gamePreference)
    {
        _inputHandler = inputHandler;
        _crossWord = crossWord;
        _inGameMenu = inGameMenu;
        _windowsController = windowsController;
        _gamePreference = gamePreference;
    }

    public override void Open()
    {
        base.Open();
        
        _gamePreference.GameStateMachine.ChangeState(_gamePreference.GameStateMachine.ActiveState);
    }

    private void OnEnable()
    {
        _inputHandler.AlwaysUpdateCall += HandleInput;
        _crossWord.OnGenerationFinish += UpdateErrorText;
    }

    private void OnDisable()
    {
        _inputHandler.AlwaysUpdateCall -= HandleInput;
        _crossWord.OnGenerationFinish -= UpdateErrorText;
    }

    private void Start()
    {
        tutorial.text = $"Generate crossword key: {_inputHandler.GenerateCrosswordKey}\nOpen pause menu: {_inputHandler.InteractWithInGameMenu}\nDelete saved crosswords: {_inputHandler.DeleteSaves}";
    }

    private void UpdateErrorText(string error)
    {
        errorText.text = error;
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(_inputHandler.GenerateCrosswordKey))
        {
            //Debug.Log("Generate Crossword\n");
            _crossWord.StartGenerate();
            SetDescription(_crossWord.SpawnedWords, _inGameMenu.IsHintsEnabled);
        }

        if (_inputHandler.RightMouseButton)
        {
            _windowsController.OpenWindow("GameUI");
        }
    }
    
    public void UpdateDescription(HashSet<CharacterLogic> spawnedWords)
    {
        SetDescription(spawnedWords, _inGameMenu.IsHintsEnabled);
    }
    
    private void SetDescription(HashSet<CharacterLogic> spawnedWords, bool hints)
    {
        string horizontalWordDesc = "Horizontal:\n", verticalWordDesc = "Vertical:\n";

        wordDescriptions.text = string.Empty;

        int i = 0, f = 0;
        foreach (CharacterLogic word in spawnedWords)
        {
            if (word.WordData.Orientation == WordOrientation.horizontal)
            {
                if (hints == true)
                    horizontalWordDesc += $"{i + 1}) {word.WordData.WordDescription} ({word.WordData.Word})\n";
                else
                    horizontalWordDesc += $"{i + 1}) {word.WordData.WordDescription}\n";
                
                i++;
            }
            else
            {
                if (hints == true)
                    verticalWordDesc += $"{f + 1}) {word.WordData.WordDescription} ({word.WordData.Word})\n";
                else
                    verticalWordDesc += $"{f + 1}) {word.WordData.WordDescription}\n";
                
                f++;
            }
        }
        wordDescriptions.text = $"{horizontalWordDesc} \n {verticalWordDesc}";
        wordDescriptions.text = wordDescriptions.text.Replace("\r", "");
    }
}