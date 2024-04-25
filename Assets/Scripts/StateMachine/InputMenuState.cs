public class InputMenuState : GameState
{
    private readonly InputHandler _inputHandler;
    
    public InputMenuState (InputHandler inputHandler)
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
