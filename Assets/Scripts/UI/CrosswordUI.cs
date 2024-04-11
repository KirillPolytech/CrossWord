using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CrosswordUI : MonoBehaviour
{
    [SerializeField] private GameObject[] ui;
    [SerializeField] private TextMeshProUGUI wordDescriptions;
    [SerializeField] private TMP_InputField wordInputMenu;
    [SerializeField] private Button generateCrosswordButton;

    private int _wordLength = 0;
    private CharacterLogic _characterLogic;
    private bool _isInputMenuOpened = false;
    
    private InputHandler _inputHandler;
    private CrossWord _crossWord;
    private InGameMenu _inGameMenu;
    private GamePreference _gamePreference;
    private void Awake()
    {
        _inputHandler = FindAnyObjectByType<InputHandler>();
        _crossWord = FindAnyObjectByType<CrossWord>();
        _inGameMenu = FindAnyObjectByType<InGameMenu>();
        _gamePreference = FindAnyObjectByType<GamePreference>();
        
        generateCrosswordButton.onClick.AddListener( () =>
        {
            _crossWord.Generate();
            SetDescription(_crossWord.SpawnedWords, _inGameMenu.IsHintsEnabled);
        });
    }

    private void Start()
    {
        wordInputMenu.gameObject.SetActive(false);

        ui = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            ui[i] = transform.GetChild(i).gameObject;
        }
    }

    private void Update()
    {
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

        wordInputMenu.onValueChanged.RemoveAllListeners();

        wordInputMenu.gameObject.SetActive(true);

        _wordLength = logic.WordData.Characters.Length;

        CharacterData[] cachedData = logic.WordData.Characters;

        UnityEvent<int, string> e = new UnityEvent<int, string>();
        e.AddListener(LimitText);

        UnityEvent<string, CharacterData[]> f = new UnityEvent<string, CharacterData[]>();
        f.AddListener(UpdateText);

        wordInputMenu.onValueChanged.AddListener( (s) => { e.Invoke(_wordLength, s); f.Invoke(s, cachedData); logic.CheckWordCompletion();  });
        
        _gamePreference.GameStateMachine.ChangeState(_gamePreference.GameStateMachine.PauseState);

        return true;
    }

    public void CloseInputPanel()
    {
        if (_characterLogic == true)
            _characterLogic.ChangeWordColor(ColorType.normal);

        wordInputMenu.onValueChanged.RemoveAllListeners();

        wordInputMenu.gameObject.SetActive(false);

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
    
    private void UpdateText(string s, CharacterData[] d)
    {
        for (int i = 0; i < d.Length; i++)
        {
            if (i >= s.Length)
            {
                d[i].CurrentChar.text = "";
                continue;
            }

            d[i].CurrentChar.text = s[i].ToString().ToLower();
        }
    }

    private void LimitText(int wordLength, string s)
    {
        if (s == null || s.Length <= 0)
            return;

        int value = Mathf.Clamp(wordLength, 0, wordLength);
        value = Mathf.Clamp(value, 0, s.Length);
        wordInputMenu.text = s.Substring(0, value);
    }
}