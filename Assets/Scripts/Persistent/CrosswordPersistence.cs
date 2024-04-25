using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

public class CrosswordPersistence
{
    public string chosenCrossword;
    public string chosenDescription;

    private readonly CustomCrossword[] _crosswords;
    private int _ind = 0;
    private readonly string _key = "k";

    public CrosswordPersistence()
    {
        //FileStream file = File.Create(Application.persistentDataPath + "/MySaveData.txt");

        /*
        for (int i = 0; i < _crosswords.Length; i++)
        {
            _crosswords[i] = new CustomCrossword();
            for (int j = 0; j < 15; j++)
            {
                _crosswords[i]._words.Add(new WordData
                {
                    WordIndex = 0,
                    Word = Random.Range(0,255).ToString(),
                    WordDescription = Random.Range(0,255).ToString(),
                });
            }
        }
        */
       
        string str = PlayerPrefs.GetString(_key);
        _crosswords = JsonConvert.DeserializeObject<CustomCrossword[]>(str);

        if (_crosswords == null)
        {
            _crosswords = new CustomCrossword[2];
        }
        
        Debug.Log($"{str.Length * sizeof(char)}");
    }

    public void SaveCrossword(CustomCrossword customCrossword)
    {
        _crosswords[_ind++] = customCrossword;
        DeleteData();
        SaveData();
    }

    public List<CustomCrossword> LoadCrosswords()
    {
        return _crosswords.Where(x => x != null).ToList();
    }

    private void SaveData()
    {
        string jsonArray = JArray.FromObject(_crosswords).ToString();
        PlayerPrefs.SetString(_key, jsonArray);
        PlayerPrefs.Save();
    }

    private void DeleteData()
    {
        PlayerPrefs.DeleteAll();
    }
}