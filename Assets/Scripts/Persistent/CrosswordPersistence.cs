using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class CrosswordPersistence : ITickable
{
    public string chosenCrossword;
    public string chosenDescription;

    private readonly List<CustomCrossword> _crosswords = new List<CustomCrossword>();
    private int _ind;
    private readonly string _key = "k";
    private InputHandler _inputHandler;
    
    [Inject]
    public CrosswordPersistence(InputHandler inputHandler)
    {
        _inputHandler = inputHandler; 
        
        string stringFromBrowser = PlayerPrefs.GetString(_key);

        CustomCrossword dataFromBrowser = JsonUtility.FromJson<CustomCrossword>(stringFromBrowser);

        if (dataFromBrowser != null)
            _crosswords.Add(dataFromBrowser);

        Debug.Log(dataFromBrowser == null ? $"data null" : $"Data is not null");
    }
    
    [Inject]
    public void Tick()
    {
        if (Input.GetKeyDown(_inputHandler.DeleteSaves))
        {
            DeleteData();
        }
    }

    public void SaveCrossword(CustomCrossword customCrossword)
    {
        _crosswords.Add(customCrossword);
        
        DeleteData();
        SaveData();
    }

    public List<CustomCrossword> LoadCrosswords()
    {
        return _crosswords?.Where(x => x != null).ToList();
    }

    private void SaveData()
    {
        string jsonArray = JsonUtility.ToJson(_crosswords[0]);
        Debug.Log($"Data to array");
        PlayerPrefs.SetString(_key, jsonArray);
        Debug.Log($"SetData jsonArray: {jsonArray}");
        PlayerPrefs.Save();
        Debug.Log($"saves data");
    }

    public void DeleteData()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Data deleted");
    }
}
//FileStream file = File.Create(Application.persistentDataPath + "/MySaveData.txt");

//string jsonArray = JArray.FromObject(_crosswords).ToString();

//_crosswords = JsonConvert.DeserializeObject<CustomCrossword[]>(dataFromBrowser);