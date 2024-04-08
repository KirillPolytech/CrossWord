using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private Button[] buttons;

    private GamePreference _gamePreference;
    private CrosswordFilesStorage _crosswordFilesStorage;
    private void Awake()
    {
        _gamePreference = FindAnyObjectByType<GamePreference>();
        _crosswordFilesStorage = FindAnyObjectByType<CrosswordFilesStorage>();
        
        for (int i = 0; i < buttons.Length; i++)
        {
            int ind = i;
            buttons[i].onClick.AddListener(() =>
            {
                SendCrossword(ind);
                SceneLoader.LoadScene(0);
            });
        }
    }

    private void SendCrossword(int i)
    {
        _gamePreference.ChoosenCrossword = _crosswordFilesStorage.CrosswordFiles[i].Words;
        _gamePreference.ChoosenDescription = _crosswordFilesStorage.CrosswordFiles[i].Description;
    }
}
