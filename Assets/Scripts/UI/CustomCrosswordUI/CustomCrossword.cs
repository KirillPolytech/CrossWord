using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class CustomCrossword
{
    public List<WordData> words = new List<WordData>();

    public void AddWord(string word, string description)
    {
        WordData data = new WordData
        {
            Word = word,
            WordDescription = description
        };
        words.Add(data);
    }

    public string GetWords() => words.Aggregate("", (current, word) => current + word.Word + "\n");

    public string GetWordDescriptions() =>  words.Aggregate("", (current, word) => current + word.WordDescription + "\n");
}