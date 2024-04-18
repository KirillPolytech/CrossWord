using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CrosswordUI : MonoBehaviour
{
    [SerializeField] private GameObject[] ui;
    [SerializeField] private TextMeshProUGUI tutorial;
    [SerializeField] private TextMeshProUGUI wordDescriptions;
    [SerializeField] private TMP_InputField wordInputMenu;
    [SerializeField] private Button closeButton;

    private int _wordLength = 0;
    private CharacterLogic _characterLogic;
    private bool _isInputMenuOpened = false;
    
    private InputHandler _inputHandler;
    private CrossWord _crossWord;
    private InGameMenu _inGameMenu;
    private GamePreference _gamePreference;

    [Inject]
    public void Construct(InputHandler inputHandler, CrossWord crossWord, InGameMenu inGameMenu, GamePreference gamePreference)
    {
        _inputHandler = inputHandler;
        _crossWord = crossWord;
        _inGameMenu = inGameMenu;
        _gamePreference = gamePreference;

        tutorial.text = $"Generate crossword key: {_inputHandler.GenerateCrosswordKey}";
    }

    private void OnEnable()
    {
        _inputHandler.AlwaysUpdateCall += HandleInput;
    }

    private void OnDisable()
    {
        _inputHandler.AlwaysUpdateCall -= HandleInput;
    }

    private void Start()
    {
        wordInputMenu.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);

        ui = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            ui[i] = transform.GetChild(i).gameObject;
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(_inputHandler.GenerateCrosswordKey))
        {
            _crossWord.Generate();
            SetDescription(_crossWord.SpawnedWords, _inGameMenu.IsHintsEnabled);
        }
        
        if (Input.GetKeyDown(_inputHandler.HideInGameUIKey))
        {
            foreach (var t in ui)
            {
                t.SetActive(!t.activeSelf);
            }
        }   

        if (_inputHandler.RightMouseButton)
        {
            CloseInputPanel();
        }
    }

    public bool OpenInputPanel(CharacterLogic logic)
    {
        if (_isInputMenuOpened == true)
            return false;

        _isInputMenuOpened = true;

        _characterLogic = logic;

        wordInputMenu.text = "";
        for (int i = 0; i < logic.WordData.Characters.Length; i++)
        {
            wordInputMenu.text += logic.WordData.Characters[i].CurrentChar.text;
        }

        wordInputMenu.onValueChanged.RemoveAllListeners();

        wordInputMenu.gameObject.SetActive(true);
        closeButton.gameObject.SetActive(true);

        _wordLength = logic.WordData.Characters.Length;

        CharacterData[] cachedData = logic.WordData.Characters;

        wordInputMenu.onValueChanged.AddListener((s) =>
        {
            TextTools.LimitText(_wordLength, s, wordInputMenu); 
            TextTools.UpdateText(s, cachedData); 
            logic.CheckWordCompletion();
        });
        
        _gamePreference.GameStateMachine.ChangeState(_gamePreference.GameStateMachine.InputMenuState);

        return true;
    }

    public void CloseInputPanel()
    {
        if (_characterLogic == true)
            _characterLogic.ChangeWordColor(ColorType.normal);

        wordInputMenu.onValueChanged.RemoveAllListeners();

        wordInputMenu.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);

        wordInputMenu.text = string.Empty;

        _isInputMenuOpened = false;
        
        _gamePreference.GameStateMachine.ChangeState(_gamePreference.GameStateMachine.ActiveState);
    }
    
    private void SetDescription(HashSet<CharacterLogic> spawnedWords, bool hints)
    {
        string horizontalWordDesc = "Horizontal:\n", verticalWordDesc = "Vertical:\n";       

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
                if (hints)
                    verticalWordDesc += $"{f + 1}) {word.WordData.WordDescription} ({word.WordData.Word})\n";
                else
                    verticalWordDesc += $"{f + 1}) {word.WordData.WordDescription}\n";
                
                f++;
            }
        }
        wordDescriptions.text = $"{horizontalWordDesc} \n {verticalWordDesc}";
    }

    public void UpdateDescription( )
    {
        SetDescription(_crossWord.SpawnedWords, _inGameMenu.IsHintsEnabled);
    }
}