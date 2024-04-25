using UnityEngine;
using Zenject;

public class MenuInstaller : MonoInstaller
{
    [SerializeField] private InputWordPairsScrollView inputWordPairsScroll;
    [SerializeField] private CustomCrosswordScrollView crosswordScroll;
    
    public override void InstallBindings()
    {
        Container.Bind<InputWordPairsScrollView>().FromInstance(inputWordPairsScroll).AsSingle();
        Container.Bind<CustomCrosswordScrollView>().FromInstance(crosswordScroll).AsSingle();
    }
}
