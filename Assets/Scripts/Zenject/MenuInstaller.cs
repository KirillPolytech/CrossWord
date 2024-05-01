using UnityEngine;
using Zenject;

public class MenuInstaller : MonoInstaller
{
    [SerializeField] private InputWordPairsScrollView inputWordPairsScroll;
    [SerializeField] private CustomCrosswordScrollView crosswordScroll;
    [SerializeField] private WindowsController windowsController;
    
    public override void InstallBindings()
    {
        Container.BindInstance(inputWordPairsScroll).AsSingle();
        Container.BindInstance(crosswordScroll).AsSingle();
        Container.BindInstance(windowsController).AsSingle();
        Container.BindInterfacesAndSelfTo<CustomCrosswordsButtonController>().AsSingle();
    }
}
