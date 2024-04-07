using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CrosswordUI : MonoBehaviour
{
    [SerializeField] private GameObject[] UI;
    [SerializeField] private TextMeshProUGUI wordDescriptions;
    [SerializeField] private CrosswordData crosswordData;
    [SerializeField] private TMP_InputField wordInputMenu;

    private string[] _descriptions;
    private int _wordLength = 0;
    private CharacterLogic _characterLogic;
    private bool _isInputMenuOpened = false;
    private InputHandler _inputHandler;
    private void Awake()
    {
        _inputHandler = FindAnyObjectByType<InputHandler>();    

        wordInputMenu.gameObject.SetActive(false);

        UI = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            UI[i] = transform.GetChild(i).gameObject;
        }

        _descriptions = crosswordData.descriptions.text.Split("\r\n");
    }

    private void Update()
    {
        if (Input.GetKeyDown(_inputHandler.HideInGameUIKey))
        {
            foreach (var t in UI)
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

        _wordLength = logic.wordData.characters.Length;

        CharacterData[] cachedData = logic.wordData.characters;

        UnityEvent<int, string> e = new UnityEvent<int, string>();
        e.AddListener(LimitText);

        UnityEvent<string, CharacterData[]> f = new UnityEvent<string, CharacterData[]>();
        f.AddListener(UpdateText);

        wordInputMenu.onValueChanged.AddListener( (s) => { e.Invoke(_wordLength, s); f.Invoke(s, cachedData); logic.CheckWordCompletion();  });

        return true;
    }

    public void CloseInputPanel()
    {
        if (_characterLogic != null)
            _characterLogic.ChangeWordColor(ColorType.normal);

        wordInputMenu.onValueChanged.RemoveAllListeners();

        wordInputMenu.gameObject.SetActive(false);

        wordInputMenu.text = string.Empty;

        _isInputMenuOpened = false;
    }

    private void UpdateText(string s, CharacterData[] d)
    {
        for (int i = 0; i < d.Length; i++)
        {
            if (i >= s.Length)
            {
                d[i].currentChar.text = "";
                continue;
            }

            d[i].currentChar.text = s[i].ToString().ToLower();
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

    public void SetDescription(HashSet<CharacterLogic> spawnedWords)
    {
        string horizontalWordDesc = "Horizontal:\n", verticalWordDesc = "Vertical:\n";       

        int i = 0, f = 0;
        foreach (CharacterLogic character in spawnedWords)
        {
            if (character.wordData.orientation == WordOrientation.horizontal)
            {
                horizontalWordDesc += $"{i + 1}) {_descriptions[character.wordData.wordIndex]}\n";
                i++;
            }
            else
            {
                verticalWordDesc += $"{f + 1}) {_descriptions[character.wordData.wordIndex]}\n";
                f++;
            }
        }
        wordDescriptions.text = $"{horizontalWordDesc} \n {verticalWordDesc}";
    }
}