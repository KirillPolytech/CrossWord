using TMPro;
using UnityEngine;

public class CharacterLogic : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;

    public CharacterData characterData;
    public WordData wordData;
    private void Awake()
    {
        characterData = new CharacterData();

        characterData.inputField = inputField;

        inputField.onValueChanged.AddListener((s) => LimitText(s));
    }

    private void LimitText(string s)
    {
        s = s.Trim();
        if (s == null || s.Length <= 0)
            return;

        s = s[0].ToString();

        inputField.text = s;
    }

}
