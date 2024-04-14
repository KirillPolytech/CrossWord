using UnityEngine;
using Zenject;

public class MainInstaller : MonoInstaller
{
    [SerializeField] private CrossWord crosswordInstance;
    [SerializeField] private InGameMenu inGameMenuInstance;
    public override void InstallBindings()
    {
        Container.Bind<InputHandler>().FromInstance(FindAnyObjectByType<InputHandler>()).AsSingle();
        
        Container.Bind<CrossWord>().FromInstance(crosswordInstance).AsSingle();
        Container.Bind<InGameMenu>().FromInstance(inGameMenuInstance).AsSingle();
        Container.Bind<GamePreference>().FromInstance(FindAnyObjectByType<GamePreference>()).AsSingle();
    }
}
