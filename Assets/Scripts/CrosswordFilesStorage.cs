using System;
using UnityEngine;

public class CrosswordFilesStorage : MonoBehaviour
{
    public CrosswordFileData[] CrosswordFiles;
}
[Serializable]
public struct CrosswordFileData
{
    public TextAsset Words;
    public TextAsset Description;
}
