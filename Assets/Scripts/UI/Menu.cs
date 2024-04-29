using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Menu : MonoBehaviour
{
    [SerializeField] private Button[] choiceButtons;
    [SerializeField] private Button addCrosswordButton;
    [SerializeField] private Button deleteCrosswordButton;

    private CrosswordPersistence _crosswordPersistence;
    private CustomCrosswordsStorage _customCrosswordsStorage;
    private CrosswordFilesStorage _crosswordFilesStorage;

    [Inject]
    public void Construct(CrosswordFilesStorage crosswordFilesStorage, CrosswordPersistence crosswordPersistence, CustomCrosswordsStorage customCrosswordsStorage)
    {
        _crosswordPersistence = crosswordPersistence;
        _customCrosswordsStorage = customCrosswordsStorage;
        _crosswordFilesStorage = crosswordFilesStorage;
    }
    
    private void Start()
    {
        addCrosswordButton.onClick.AddListener(_customCrosswordsStorage.Create);
        deleteCrosswordButton.onClick.AddListener(_customCrosswordsStorage.Delete);
        
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            int ind = i;
            choiceButtons[i].onClick.AddListener(() =>
            {
                SendCrossword(ind);
                SceneLoader.LoadScene(SceneLoader.MainScene);
            });
        }
    }
    
    private void SendCrossword(int i)
    {
        _crosswordPersistence.chosenCrossword = _crosswordFilesStorage.CrosswordFiles[i].Words.ToString();
        _crosswordPersistence.chosenDescription = _crosswordFilesStorage.CrosswordFiles[i].Description.ToString();
    }
}
