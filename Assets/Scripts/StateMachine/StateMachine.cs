// ReSharper disable All
public class StateMachine 
{
    public readonly PauseState PauseState;
    public readonly ActiveState ActiveState;

    private GameState _currentState;
    public StateMachine(InputHandler inputHandler)
    {
        PauseState = new PauseState(inputHandler);
        ActiveState = new ActiveState(inputHandler);

        _currentState = ActiveState;
    }

    public void ChangeState(GameState state)
    {
        _currentState.ExitState();
        _currentState = state;
        _currentState.EnterState();
    }
}
