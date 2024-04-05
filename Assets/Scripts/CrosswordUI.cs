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

    public KeyCode HideUIKey = KeyCode.F1;

    private string[] descriptions;
    private int wordLength = 0;
    private CharacterLogic characterLogic;
    private bool _isInputMenuOpened = false;
    private void Awake()
    {
        wordInputMenu.gameObject.SetActive(false);

        UI = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            UI[i] = transform.GetChild(i).gameObject;
        }

        descriptions = crosswordData.descriptions.text.Split("\r\n");
    }

    private void Update()
    {
        if (Input.GetKeyDown(HideUIKey))
        {
            for (int i = 0; i < UI.Length; i++)
            {
                UI[i].SetActive(!UI[i].activeSelf);
            }
        }   

        if (Input.GetMouseButtonDown(1))
        {
            CloseInputPanel();
        }
    }

    public bool OpenInputPanel(CharacterLogic logic)
    {
        if (_isInputMenuOpened == true)
            return false;

        _isInputMenuOpened = true;

        characterLogic = logic;

        wordInputMenu.onValueChanged.RemoveAllListeners();

        wordInputMenu.gameObject.SetActive(true);

        wordLength = logic.wordData.characters.Length;

        CharacterData[] cachedData = logic.wordData.characters;

        UnityEvent<int, string> e = new UnityEvent<int, string>();
        e.AddListener((d, s) => LimitText(d, s));

        UnityEvent<string, CharacterData[]> f = new UnityEvent<string, CharacterData[]>();
        f.AddListener((s, d) => UpdateText(s, d));

        wordInputMenu.onValueChanged.AddListener( (s) => { e.Invoke(wordLength, s); f.Invoke(s, cachedData); logic.CheckWordCompletion();  });

        return true;
    }

    public void CloseInputPanel()
    {
        if (characterLogic != null)
            characterLogic.ChangeWordColor(ColorType.normal);

        wordInputMenu.onValueChanged.RemoveAllListeners();

        wordInputMenu.gameObject.SetActive(false);

        wordInputMenu.text = string.Empty;

        _isInputMenuOpened = false;
    }

    public void UpdateText(string s, CharacterData[] d)
    {
        for (int i = 0; i < d.Length; i++)
        {
            if (i >= s.Length)
            {
                d[i].text.text = "";
                continue;
            }

            d[i].text.text = s[i].ToString().ToLower();
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
                horizontalWordDesc += $"{i + 1}) {descriptions[character.wordData.wordIndex]}\n";
                i++;
            }
            else
            {
                verticalWordDesc += $"{f + 1}) {descriptions[character.wordData.wordIndex]}\n";
                f++;
            }
        }

        wordDescriptions.text = $"{horizontalWordDesc} \n {verticalWordDesc}";
    }
}
