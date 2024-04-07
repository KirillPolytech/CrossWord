using UnityEngine;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour
{
    [SerializeField] private Canvas inGameMenu;
    [SerializeField] private Button continueButton;

    private InputHandler _inputHandler;
    private GamePreference _gamePreference;
    private void Awake()
    {
        _inputHandler = FindFirstObjectByType<InputHandler>();
        _gamePreference = FindFirstObjectByType<GamePreference>();

        inGameMenu.enabled = false;

        _inputHandler.UpdateCall += UpdateInGameMenuState;
        
        continueButton.onClick.AddListener(ChangeInGameMenuState);
    }

    private void UpdateInGameMenuState()
    {
        if (Input.GetKeyDown(_inputHandler.InteractInGameMenu) == false)
            return;

        ChangeInGameMenuState();
    }

    private void ChangeInGameMenuState()
    {
        inGameMenu.enabled = !inGameMenu.enabled;
        if (inGameMenu.enabled)
            _gamePreference.GameStateMachine.ChangeState(_gamePreference.GameStateMachine.PauseState);
        else
            _gamePreference.GameStateMachine.ChangeState(_gamePreference.GameStateMachine.ActiveState);
    }
}
