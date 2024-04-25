using TMPro;
using UnityEngine;

public class WordsInputField : MonoBehaviour
{
    /*
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private InputHandler inputHandler;

    private void Awake()
    {
        inputField.onSelect.AddListener(s => inputHandler.AlwaysUpdateCall += HandleWordOrder);
        inputField.onDeselect.AddListener(s => inputHandler.AlwaysUpdateCall -= HandleWordOrder);
    }

    private void OnDestroy()
    {
        inputField.onDeselect.RemoveListener(s => inputHandler.AlwaysUpdateCall -= HandleWordOrder);
    }

    private void HandleWordOrder()
    {
        ProcessWordOrder(inputHandler.Space);
    }

    private void ProcessWordOrder(bool isEnterPressed)
    {
        if (isEnterPressed != true) 
            return;
        
        inputField.text += "\n\r";
        inputField.caretPosition = inputField.text.Length;
    }
    */
}
