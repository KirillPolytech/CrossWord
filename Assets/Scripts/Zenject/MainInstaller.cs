using UnityEngine;
using Zenject;

public class MainInstaller : MonoInstaller
{
    [SerializeField] private CrossWord crosswordInstance;
    [SerializeField] private InGameMenu inGameMenuInstance;
    public override void InstallBindings()
    {
        Container.Bind<CrossWord>().FromInstance(crosswordInstance).AsSingle();
        Container.Bind<InGameMenu>().FromInstance(inGameMenuInstance).AsSingle();
    }
}
