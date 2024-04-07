using UnityEngine;

public class CharacterLogic : MonoBehaviour
{   
    public WordData wordData = new WordData();
    public string wordDescription;
    public bool IsCompleted { get; private set; }

    private CrosswordUI UI;
    private void Awake()
    {
        UI = FindAnyObjectByType<CrosswordUI>();
    }

    private void Start()
    {
        IsCompleted = false;
    }

    public void OpenMenu()
    {
        if (IsCompleted == true)
            return;

        bool isOpened = UI.OpenInputPanel(this);

        if (isOpened == true)
            ChangeWordColor(ColorType.selected);
    }

    public void ChangeWordColor(ColorType color)
    {
        if (IsCompleted == true) 
            return;

        Color col;
        switch (color)
        {
            case ColorType.selected:
                col = Color.yellow;
                break;
            case ColorType.finished:
                col = Color.green;
                break;
            case ColorType.normal:
                col = Color.white;
                break;
            default: 
                col = Color.white;
                break;
        }

        foreach (var c in wordData.characters)
        {
            c.meshRenderer.material.color = col;
        }
    }

    public void CheckWordCompletion()
    {
        if (IsCompleted == true)
            return;

        foreach (var item in wordData.characters)
        {
            if (item == null)
                continue;

            string str = item.desiredChar.ToString().Trim();
            string str2 = item.currentChar.text.ToString().Trim();

            //Debug.Log($"InputField: {str2} DesiredChar: {str}");

            if (str2 != str)
            {
                return;
            }                
        }

        foreach (var item in wordData.characters)
        {
            if (item == null)
                continue;
            
            item.currentChar.color = Color.green;
        }

        IsCompleted = true;

        UI.CloseInputPanel();

        ChangeWordColor(ColorType.finished);

        Debug.Log("Completed");
    }
}