using System.Collections.Generic;
using System.Linq;
using Zenject;

public class CustomCrosswordsButtonController : IInitializable
{
    private InputWordPairsScrollView _inputWordPairsScroll;
    private CustomCrosswordScrollView _crosswordScroll;
    private CrosswordPersistence _crosswordPersistence;

    [Inject]
    public void Construct(
        InputWordPairsScrollView inputWordPairsScroll, 
        CustomCrosswordScrollView crosswordScroll, 
        CrosswordPersistence crosswordPersistence)
    {
        _inputWordPairsScroll = inputWordPairsScroll;
        _crosswordScroll = crosswordScroll;
        _crosswordPersistence = crosswordPersistence;
    }
    
    public void Initialize()
    {
        RestoreCrosswords(_crosswordPersistence.LoadCrosswords());
    }

    public void Create()
    {
        if (_crosswordPersistence.CurrentIndex > _crosswordPersistence.CrosswordLIMITS - 1 || 
            _crosswordScroll.CustomCrosswordsButtonsCount > _crosswordPersistence.CrosswordLIMITS - 1)
            return;
        
        _crosswordScroll.CreateCustomCrosswordButton(() => 
            _inputWordPairsScroll.Initialize(_crosswordPersistence.CurrentIndex));
    }
    
    private void RestoreCrosswords(List<List<WordData>> data)
    {
        if (data == null)
            return;
        
        for (int i = 0; i < data.Count; i++)
        {
            int ind = i;
            _crosswordScroll.CreateCustomCrosswordButton(() =>
            {
                _inputWordPairsScroll.Initialize(ind);
                _inputWordPairsScroll.RestoreInputFields(data.ElementAt(ind));
            }
            );
        }
    }

    public void Delete()
    {
        _crosswordScroll.DeleteCustomCrosswordButton();

        _crosswordPersistence.DeleteData();
    }
}