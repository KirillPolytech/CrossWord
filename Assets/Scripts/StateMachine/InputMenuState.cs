public class InputMenuState : GameState
{
    private readonly InputHandler _inputHandler;
    private bool _tempState;
    public InputMenuState (InputHandler inputHandler)
    {
        _inputHandler = inputHandler;
    }
    public void EnterState()
    {
        _tempState = _inputHandler.IsEnabled;
        
        _inputHandler.IsEnabled = false;
    }

    public void ExitState()
    {
        _inputHandler.IsEnabled = _tempState;
    }
}
