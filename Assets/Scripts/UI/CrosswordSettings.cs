using TMPro;
using UnityEngine;

public class CrosswordSettings : MonoBehaviour
{
    [SerializeField] private TMP_InputField generateCrosswordInputField;

    private InputHandler _input;
    private void Awake()
    {
        _input = FindAnyObjectByType<InputHandler>();
        
        generateCrosswordInputField.onSelect.AddListener((s) => _input.AlwaysUpdateCall += UpdateInputFields);
        generateCrosswordInputField.onDeselect.AddListener((s) => _input.AlwaysUpdateCall -= UpdateInputFields );
        
        generateCrosswordInputField.text = _input.GenerateCrosswordKey.ToString();
    }

    private void OnDisable()
    {
        _input.AlwaysUpdateCall -= UpdateInputFields;
    }

    private void UpdateInputFields()
    {
        generateCrosswordInputField.text = _input.CurrentKeyDown.ToString();
        _input.GenerateCrosswordKey = _input.CurrentKeyDown;
    }
}
