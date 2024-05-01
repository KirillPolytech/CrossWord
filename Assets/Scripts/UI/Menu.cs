using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Menu : MonoBehaviour
{
    [SerializeField] private Button[] choiceButtons;
    [SerializeField] private Button addCrosswordButton;
    [SerializeField] private Button deleteCrosswordButton;

    private CrosswordPersistence _crosswordPersistence;
    private CustomCrosswordsButtonController _customCrosswordsButtonController;
    private CrosswordFilesStorage _crosswordFilesStorage;

    [Inject]
    public void Construct(
        CrosswordFilesStorage crosswordFilesStorage, 
        CrosswordPersistence crosswordPersistence, 
        CustomCrosswordsButtonController customCrosswordsButtonController)
    {
        _crosswordPersistence = crosswordPersistence;
        _customCrosswordsButtonController = customCrosswordsButtonController;
        _crosswordFilesStorage = crosswordFilesStorage;
    }
    
    private void Start()
    {
        addCrosswordButton.onClick.AddListener(_customCrosswordsButtonController.Create);
        deleteCrosswordButton.onClick.AddListener(_customCrosswordsButtonController.Delete);
        
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
