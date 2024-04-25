using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class InGameMenu : MonoBehaviour
{
    [SerializeField] private Canvas inGameMenu;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button hintButton;
    [SerializeField] private TextMeshProUGUI hintButtonText;
    [SerializeField] private Button exitButton;

    public bool IsHintsEnabled { get; private set; }

    private InputHandler _inputHandler;
    private GamePreference _gamePreference;
    private CrosswordUI _crosswordUI;
    private GameState _tempState;

    [Inject]
    public void Construct(GamePreference gamePreference, InputHandler inputHandler)
    {
        _gamePreference = gamePreference;
        _inputHandler = inputHandler;
    }
    
    private void Awake()
    {
        _crosswordUI = FindAnyObjectByType<CrosswordUI>();

        inGameMenu.enabled = false;
        
        hintButton.onClick.AddListener(_crosswordUI.UpdateDescription);
        continueButton.onClick.AddListener(ChangeInGameMenuState);
        exitButton.onClick.AddListener(() => SceneLoader.LoadScene(SceneLoader.MenuScene));
        
        hintButtonText.text = "Hints: off";
    }

    private void OnEnable()
    {
        _inputHandler.AlwaysUpdateCall += UpdateInGameMenuState;
    }

    private void OnDisable()
    {
        _inputHandler.AlwaysUpdateCall -= UpdateInGameMenuState;
    }

    private void UpdateInGameMenuState()
    {
        if (Input.GetKeyDown(_inputHandler.InteractInGameMenu) == false) //|| _gamePreference.GameStateMachine.CurrentState == _gamePreference.GameStateMachine.PauseState)
            return;

        ChangeInGameMenuState();
    }

    private void ChangeInGameMenuState()
    {
        inGameMenu.enabled = !inGameMenu.enabled;

        if (inGameMenu.enabled is true)
        {
            _tempState = _gamePreference.GameStateMachine.CurrentState;
            _gamePreference.GameStateMachine.ChangeState(_gamePreference.GameStateMachine.PauseState);
        }
        else
        {
            _gamePreference.GameStateMachine.ChangeState(_tempState);
        }
    }
    
    public void ChangeHintsState()
    {
        IsHintsEnabled = !IsHintsEnabled;

        string str;
        str = IsHintsEnabled == true ? "on" : "off";
        hintButtonText.text = $"Hints: {str}";
    }
}
