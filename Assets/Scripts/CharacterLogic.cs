using UnityEngine;

public class CharacterLogic : MonoBehaviour
{   
    public WordData wordData = new WordData();
    public string wordDescription;
    public bool IsCompleted { get; private set; }
    private void Awake()
    {
        IsCompleted = false;
    }

    public void CheckWordCompletion()
    {
        if (IsCompleted == true)
            return;

        foreach (var item in wordData.characters)
        {
            if (item?.inputField.text != item?.desiredChar.ToString())
                return;         
        }

        foreach (var item in wordData.characters)
        {
            if (item != null && item.inputField)
                item.inputField.textComponent.color = Color.green;
        }

        IsCompleted = true;

        Debug.Log("Completed");
    }
}