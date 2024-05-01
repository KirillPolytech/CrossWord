using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class CrosswordPersistence : ITickable
{
    public string chosenCrossword;
    public string chosenDescription;

    public int CurrentIndex { get; private set; }

    public int CrosswordLIMITS => CrosswordsLimit;
    
    private const int CrosswordsLimit = 5;
    private readonly CustomCrossword[] _crosswords = new CustomCrossword[CrosswordsLimit];
    private InputHandler _inputHandler;

    private static readonly string[] Keys = new string[]{"1", "2", "3", "4", "5"};
    
    [Inject]
    public CrosswordPersistence(InputHandler inputHandler)
    {
        _inputHandler = inputHandler;

        for (int i = 0; i < CrosswordsLimit; i++)
        {
            string stringFromBrowser = PlayerPrefs.GetString(Keys[i]);
            
            CustomCrossword dataFromBrowser = null;
            if (string.IsNullOrEmpty(stringFromBrowser) == false)
            {
                try
                {
                    dataFromBrowser = JsonUtility.FromJson<CustomCrossword>(stringFromBrowser);
                }
                catch (Exception e)
                { 
                    Debug.LogWarning(e);
                }
            }

            if (dataFromBrowser != null)
            {
                _crosswords[CurrentIndex++] = dataFromBrowser;
            }
        }

        Debug.Log(CurrentIndex == 0 ? $"data null" : $"Data is not null");
    }
    
    [Inject]
    public void Tick()
    {
        if (Input.GetKeyDown(_inputHandler.DeleteSaves))
        {
            DeleteAllData();
        }
    }

    public void SaveCrossword(CustomCrossword customCrossword, int index)
    {
        if (index > CrosswordsLimit - 1 || index < 0)
        {
            Debug.LogWarning($"Limit exceeded. {index}");
            return;
        }

        _crosswords[index] = null;
        _crosswords[index] = customCrossword;
        
        DeleteData();
        SaveData();
    }

    public List<List<WordData>> LoadCrosswords()
    {
        if (CurrentIndex <= 0 && _crosswords.All(x => x == null))
            return null;
        
        List<List<WordData>> d = new List<List<WordData>>();

        int i = 0;
        foreach (var customCrossword in _crosswords.Where(x => x != null))
        {
            List<WordData> data = new List<WordData>();
            for (int k = 0; k < customCrossword.words.Count; k++)
            {
                data.Add(customCrossword.words.ElementAt(k));
            }

            d.Add(data);
            i++;
        }

        return d;
    }

    private void SaveData()
    {
        DeleteData();
        
        for (int i = 0; i < CrosswordsLimit; i++)
        {
            if (_crosswords[i] == null)
                continue;
            
            string jsonArray = JsonUtility.ToJson(_crosswords[i]);
            PlayerPrefs.SetString(Keys[i], jsonArray);
            PlayerPrefs.Save();
            
            Debug.Log("Data saved.");
        }
    }

    public void DeleteData()
    {
        CurrentIndex = Mathf.Clamp(--CurrentIndex, 0, CrosswordLIMITS);
        PlayerPrefs.DeleteKey($"{CurrentIndex}");
        PlayerPrefs.Save();
        Debug.Log("Data deleted.");
    }

    private void DeleteAllData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}