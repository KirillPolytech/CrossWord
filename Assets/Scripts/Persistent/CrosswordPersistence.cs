using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CrosswordPersistence
{
    private void Awake()
    {
        FileStream file = File.Create(Application.persistentDataPath + "/MySaveData.txt"); 
    }

    public void SaveCrosswords()
    {
        
    }

    public List<CustomCrossword> LoadCrosswords()
    {
        return null;
    }
}
