using CrosswordWindows;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class InGameMenu : Window
{
    [SerializeField] private Button continueButton;
    [SerializeField] private Button hintButton;
    [SerializeField] private TextMeshProUGUI hintButtonText;
    [SerializeField] private Button exitButton;

    public bool IsHintsEnabled { get; private set; }

    private GameState _tempState;
    private InputHandler _inputHandler;
    private GamePreference _gamePreference;
    private CrosswordUI _crosswordUI;
    private WindowsController _windowsController;

    [Inject]
    public void Construct(
        CrossWord crossWord, 
        CrosswordUI crosswordUI, 
        GamePreference gamePreference, 
        InputHandler inputHandler, 
        WindowsController windowsController)
    {
        _gamePreference = gamePreference;
        _inputHandler = inputHandler;
        _crosswordUI = crosswordUI;
        _windowsController = windowsController;
        
        hintButton.onClick.AddListener(() => _crosswordUI.UpdateDescription(crossWord.SpawnedWords));
        continueButton.onClick.AddListener(() => _windowsController.OpenWindow("GameUI"));
        exitButton.onClick.AddListener(() => SceneLoader.LoadScene(SceneLoader.MenuScene));
    }
    
    private void OnEnable()
    {
        _inputHandler.AlwaysUpdateCall += UpdateInGameMenuState;
    }

    private void OnDisable()
    {
        _inputHandler.AlwaysUpdateCall -= UpdateInGameMenuState;
    }
    
    private void Start()
    {
        hintButtonText.text = "Hints: off";
    }
    
    public override void Open()
    {
        base.Open();
        _gamePreference.GameStateMachine.ChangeState(_gamePreference.GameStateMachine.PauseState);
    }

    private void UpdateInGameMenuState()
    {
        if (Input.GetKeyDown(_inputHandler.InteractWithInGameMenu) == false)
            return;

        _windowsController.OpenWindow("InGameWindow");
    }
    
    public void ChangeHintsState()
    {
        IsHintsEnabled = !IsHintsEnabled;

        string str;
        str = IsHintsEnabled == true ? "on" : "off";
        hintButtonText.text = $"Hints: {str}";
    }
}