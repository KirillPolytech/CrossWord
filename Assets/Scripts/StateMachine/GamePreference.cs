using UnityEngine;

public class GamePreference : MonoBehaviour
{
    public StateMachine GameStateMachine;
    public TextAsset ChoosenCrossword;
    private InputHandler _inputHandler;
    private void Awake()
    {
        _inputHandler = FindAnyObjectByType<InputHandler>();

        GameStateMachine = new StateMachine(_inputHandler);
        GameStateMachine.ChangeState(GameStateMachine.ActiveState);
    }
}
