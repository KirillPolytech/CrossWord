using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CrosswordUI : MonoBehaviour
{
    [SerializeField] private GameObject[] UI;
    [SerializeField] private TextMeshProUGUI wordDescriptions;
    [SerializeField] private CrosswordData crosswordData;

    public KeyCode HideUIKey = KeyCode.F1;

    private string[] words, descriptions;
    private void Awake()
    {
        UI = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            UI[i] = transform.GetChild(i).gameObject;
        }

        descriptions = crosswordData.descriptions.text.Split("\r\n");
    }

    private void Update()
    {
        if (Input.GetKeyDown(HideUIKey))
        {
            for (int i = 0; i < UI.Length; i++)
            {
                UI[i].SetActive(!UI[i].activeSelf);
            }
        }   
    }

    public void SetDescription(HashSet<CharacterLogic> spawnedWords)
    {
        string horizontalWordDesc = "Horizontal:\n", verticalWordDesc = "Vertical:\n";       

        int i = 0, f = 0;
        foreach (CharacterLogic character in spawnedWords)
        {
            if (character.wordData.orientation == WordOrientation.horizontal)
            {
                horizontalWordDesc += $"{i + 1}) {descriptions[character.wordData.wordIndex]}\n";
                i++;
            }
            else
            {
                verticalWordDesc += $"{f + 1}) {descriptions[character.wordData.wordIndex]}\n";
                f++;
            }
        }

        wordDescriptions.text = $"{horizontalWordDesc} \n {verticalWordDesc}";
    }
}
