using UnityEngine;
using Zenject;

public class MainInstaller : MonoInstaller
{
    [SerializeField] private CrossWord crosswordInstance;
    [SerializeField] private InGameMenu inGameMenuInstance;
    [SerializeField] private CrosswordUI crosswordUI;
    [SerializeField] private WindowsController windowsController;
    public override void InstallBindings()
    {
        Container.Bind<CrossWord>().FromInstance(crosswordInstance).AsSingle();
        Container.Bind<InGameMenu>().FromInstance(inGameMenuInstance).AsSingle();
        Container.Bind<CrosswordUI>().FromInstance(crosswordUI).AsSingle();
        Container.Bind<WindowsController>().FromInstance(windowsController).AsSingle();
    }
}
