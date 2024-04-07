public class ActiveState : GameState
{
    private readonly InputHandler _inputHandler;
    public ActiveState (InputHandler inputHandler)
    {
        _inputHandler = inputHandler;
    }
    
    public void EnterState()
    {
        _inputHandler.IsEnabled = true;
    }

    public void ExitState()
    {
        
    }
}
