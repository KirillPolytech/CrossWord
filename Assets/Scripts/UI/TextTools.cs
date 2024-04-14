using TMPro;
using UnityEngine;

public class TextTools : MonoBehaviour
{
    public static void LimitText(int wordLength, string s, TMP_InputField inputField)
    {
        if (s == null || s.Length <= 0)
            return;

        int value = Mathf.Clamp(wordLength, 0, wordLength);
        value = Mathf.Clamp(value, 0, s.Length);
        inputField.text = s[..value];
    }
    
    public static void UpdateText(string s, CharacterData[] d)
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
}
