using TMPro;
using UnityEngine;

public class CharacterData
{
    public TMP_InputField inputField;
    public Color color;
    public int charIndex;
    public Transform transform;
    public char desiredChar;
    public CharacterData()
    {
        
    }

    public CharacterData(TMP_InputField inputfield, Color c, int ind, char desiredchar, Transform t, string inputfieldText) 
    {
        inputField = inputfield;
        inputField.text = inputfieldText;

        desiredChar = desiredchar;

        color = c;
        charIndex = ind;
        transform = t;

        Subscribe();
    }

    private void Subscribe()
    {
        inputField.onValueChanged.AddListener((s) => { 
            LimitText(s);
        });
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