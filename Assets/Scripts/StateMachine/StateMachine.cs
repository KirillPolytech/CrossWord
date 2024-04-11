public class StateMachine 
{
    public GameState CurrentState { get; private set; }
    
    public readonly PauseState PauseState;
    public readonly ActiveState ActiveState;
    
    public StateMachine(InputHandler inputHandler)
    {
        PauseState = new PauseState(inputHandler);
        ActiveState = new ActiveState(inputHandler);

        CurrentState = ActiveState;
    }

    public void ChangeState(GameState state)
    {
        CurrentState.ExitState();
        CurrentState = state;
        CurrentState.EnterState();
    }
}