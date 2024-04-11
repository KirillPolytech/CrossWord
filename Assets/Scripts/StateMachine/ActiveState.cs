public class ActiveState : GameState
{
    private readonly InputHandler _inputHandler;
    
    private bool _tempState;
    public ActiveState (InputHandler inputHandler)
    {
        _inputHandler = inputHandler;
    }
    
    public void EnterState()
    {
        _tempState = _inputHandler.IsEnabled;
        
        _inputHandler.IsEnabled = true;
    }

    public void ExitState()
    {
        _inputHandler.IsEnabled = _tempState;
    }
}