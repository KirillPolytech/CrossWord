public class PauseState : GameState
{
    private readonly InputHandler _inputHandler;
    public PauseState (InputHandler inputHandler)
    {
        _inputHandler = inputHandler;
    }
    
    public void EnterState()
    {
        _inputHandler.IsEnabled = false;
    }

    public void ExitState()
    {
        
    }
}
