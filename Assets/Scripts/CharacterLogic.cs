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

        bool _isComplete = true;
        foreach (var item in wordData.characters)
        {
            string str = item?.desiredChar.ToString().Trim();
            string str2 = item?.inputField.text.ToString().Trim();
            Debug.Log($"InputField: {str2} DesiredChar: {str}");
            if (str2 != str)
            {
                //return;
                _isComplete = false;
            }                
        }

        if (_isComplete == false)
            return;

        foreach (var item in wordData.characters)
        {
            if (item != null && item.inputField)
                item.inputField.textComponent.color = Color.green;
        }

        IsCompleted = true;

        foreach (var item in wordData.characters)
        {
            item.inputField.enabled = false;
        }

        Debug.Log("Completed");
    }
}