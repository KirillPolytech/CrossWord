using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class CustomCrossword
{
    public List<WordData> _words = new List<WordData>();

    public void AddWord(string word, string description)
    {
        WordData data = new WordData
        {
            Word = word,
            WordDescription = description
        };
        _words.Add(data);
    }

    public string GetWords()
    {
        return _words.Aggregate("", (current, word) => current + word.Word + "\n");
    }

    public string GetWordDescriptions()
    {
        return _words.Aggregate("", (current, word) => current + word.WordDescription + "\n");
    }
}