using TMPro;
using UnityEngine;
using Zenject;

public class CrosswordSettings : MonoBehaviour
{
    [SerializeField] private TMP_InputField generateCrosswordInputField;

    private InputHandler _input;

    [Inject]
    public void Construct(InputHandler inputHandler)
    {
        _input = inputHandler;
    }
    
    private void Awake()
    {
        generateCrosswordInputField.onSelect.AddListener(s => _input.AlwaysUpdateCall += UpdateInputFields);
        generateCrosswordInputField.onDeselect.AddListener(s => _input.AlwaysUpdateCall -= UpdateInputFields );
        
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
