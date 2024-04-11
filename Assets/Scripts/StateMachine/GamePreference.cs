using UnityEngine;

public class GamePreference : MonoBehaviour
{
    public StateMachine GameStateMachine;
    public TextAsset ChoosenCrossword;
    public TextAsset ChoosenDescription;
    
    private InputHandler _inputHandler;
    private GamePreference _instance;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(transform.parent.gameObject);
        }
        else
        {
            Destroy(transform.parent.gameObject);
            return;
        }

        Application.targetFrameRate = 60;
        
        _inputHandler = FindAnyObjectByType<InputHandler>();

        GameStateMachine = new StateMachine(_inputHandler);
        GameStateMachine.ChangeState(GameStateMachine.ActiveState);
    }
}
