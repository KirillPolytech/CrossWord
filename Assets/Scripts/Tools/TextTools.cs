using System.Linq;
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
    
    public static int FindCharIndexInWord(string word, char chr)
    {
        if (word == null || chr == 0)
            return -1;

        for (int i = 0; i < word.Length; i++)
        {
            if (word[i] == chr)
                return i;
        }

        return -1;
    }

    public static CharacterData FindSameCharacter(CharacterData[] word, string nextWord)
    {
        return word.FirstOrDefault(t => nextWord.Any(t1 => t.DesiredChar.ToString() == t1.ToString()));
    }
}