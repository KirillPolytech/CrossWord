using CrosswordWindows;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CrosswordSettingsUI : Window
{
    [SerializeField] private TMP_InputField generateCrosswordInputField;
    [SerializeField] private Button backButton;
    [SerializeField] private TextMeshProUGUI tutorial;

    private InputHandler _input;
    private WindowsController _windowsController;

    [Inject]
    public void Construct(InputHandler inputHandler, WindowsController windowsController)
    {
        _input = inputHandler;
        _windowsController = windowsController;
    }
    
    private void Start()
    {
        generateCrosswordInputField.onSelect.AddListener(s => _input.AlwaysUpdateCall += UpdateInputFields);
        generateCrosswordInputField.onDeselect.AddListener(s => _input.AlwaysUpdateCall -= UpdateInputFields);
        backButton.onClick.AddListener(() => _windowsController.OpenWindow("Main"));
        
        generateCrosswordInputField.text = _input.GenerateCrosswordKey.ToString();

        tutorial.text = $"Open pause menu: {_input.InteractWithInGameMenu}\n" +
                        $"Close input menu: {_input.RightMouseButton}\n" +
                        $"Camera movement: WSAD\n" +
                        $"Delete all saves: {_input.DeleteSaves}\n";
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