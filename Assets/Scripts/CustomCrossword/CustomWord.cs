using TMPro;
using UnityEngine;

public struct CustomWord
{
    public GameObject Parent;
    public TMP_InputField Word;
    public TMP_InputField Description;

    public CustomWord(GameObject parent, TMP_InputField word, TMP_InputField description)
    {
        Parent = parent;
        Word = word;
        Description = description;
    }
}
