using UnityEngine;

public class StateMachine 
{
    public GameState CurrentState { get; private set; }
    
    public readonly PauseState PauseState;
    public readonly ActiveState ActiveState;
    public readonly InputMenuState InputMenuState;
    
    public StateMachine(InputHandler inputHandler)
    {
        PauseState = new PauseState(inputHandler);
        ActiveState = new ActiveState(inputHandler);
        InputMenuState = new InputMenuState(inputHandler);

        CurrentState = ActiveState;
    }

    public void ChangeState(GameState state)
    {
        CurrentState.ExitState();
        CurrentState = state;
        CurrentState.EnterState();
        
        Debug.Log($"State changed: {state}");
    }
}