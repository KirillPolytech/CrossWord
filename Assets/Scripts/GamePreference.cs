using UnityEngine;
using Zenject;

public class GamePreference
{
    public StateMachine GameStateMachine;
    
    private InputHandler _inputHandler;
    private GamePreference _instance;

    [Inject]
    public void Construct(InputHandler inputHandler)
    {
        _inputHandler = inputHandler;
        GameStateMachine = new StateMachine(_inputHandler);
        GameStateMachine.ChangeState(GameStateMachine.ActiveState);
        
        QualitySettings.vSyncCount = 2;
    }
}
