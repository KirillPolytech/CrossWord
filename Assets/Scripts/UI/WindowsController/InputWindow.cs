using CrosswordWindows;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class InputWindow : Window
{
    [SerializeField] private TMP_InputField wordInputMenu;
    [SerializeField] private Button closeButton;

    private CharacterLogic _characterLogic;
    private GamePreference _gamePreference;

    [Inject]
    public void Construct(GamePreference gamePreference)
    {
        _gamePreference = gamePreference;
    }
    public void Initialize(CharacterLogic logic)
    {
        _characterLogic = logic;
    }
    
    public override void Open()
    {
        if (_characterLogic == false)
            return;
        
        base.Open();
        
        wordInputMenu.text = "";
        int wordLength ;
        foreach (var t in _characterLogic.WordData.Characters)
        {
            wordInputMenu.text += t.CurrentChar.text;
        }

        wordInputMenu.onValueChanged.RemoveAllListeners();

        wordInputMenu.gameObject.SetActive(true);
        closeButton.gameObject.SetActive(true);

        wordLength = _characterLogic.WordData.Characters.Length;

        CharacterData[] cachedData = _characterLogic.WordData.Characters;

        wordInputMenu.onValueChanged.AddListener((s) =>
        {
            TextTools.LimitText(wordLength, s, wordInputMenu); 
            TextTools.UpdateText(s, cachedData); 
            _characterLogic.CheckWordCompletion();
        });
        
        _gamePreference.GameStateMachine.ChangeState(_gamePreference.GameStateMachine.PauseState);
    }

    public override void Close()
    {
        base.Close();
        
        wordInputMenu.onValueChanged.RemoveAllListeners();

        wordInputMenu.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);

        wordInputMenu.text = string.Empty;
        
        _gamePreference.GameStateMachine.ChangeState(_gamePreference.GameStateMachine.ActiveState);
        
        if (_characterLogic == true)
            _characterLogic.ChangeWordColor(ColorType.normal);
    }
}
