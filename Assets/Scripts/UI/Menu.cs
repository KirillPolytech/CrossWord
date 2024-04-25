using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Menu : MonoBehaviour
{
    [SerializeField] private Button[] buttons;

    private CrosswordFilesStorage _crosswordFilesStorage;

    [Inject]
    public void Construct(CrosswordFilesStorage crosswordFilesStorage)
    {
        _crosswordFilesStorage = crosswordFilesStorage;
    }
    
    private void Start()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            int ind = i;
            buttons[i].onClick.AddListener(() =>
            {
                SendCrossword(ind);
                SceneLoader.LoadScene(SceneLoader.MainScene);
            });
        }
    }
    
    private void SendCrossword(int i)
    {
        _crosswordFilesStorage.chosenCrossword = _crosswordFilesStorage.CrosswordFiles[i].Words.ToString();
        _crosswordFilesStorage.chosenDescription = _crosswordFilesStorage.CrosswordFiles[i].Description.ToString();
    }
}
