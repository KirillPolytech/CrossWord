using UnityEngine;
using Zenject;

public class GlobalInstaller : MonoInstaller
{
    [SerializeField] private DebugStuff debugStuff;
    [SerializeField] private CrosswordFilesStorage crosswordFilesStorage;
    [SerializeField] private InputHandler inputHandler;
    public override void InstallBindings()
    {
        debugStuff.Initialize();
        
        BindInputHandler();
        BindGamePreference();
        BindCrosswordFilesStorage();
        BindCustomCrosswordsStorage();
    }

    public override void Start()
    {
        SceneLoader.LoadScene(SceneLoader.MenuScene);
    }
    
    private void BindInputHandler()
    {
        Container.
            BindInstance(inputHandler)
            .AsSingle()
            .NonLazy();
    }

    private void BindGamePreference()
    {
        Container.
            Bind<GamePreference>()
            .AsSingle()
            .NonLazy();
    }

    private void BindCrosswordFilesStorage()
    {
        Container.
            BindInstance(crosswordFilesStorage).
            AsSingle().
            NonLazy();
    }
    
    private void BindCustomCrosswordsStorage()
    {
        Container.
            BindInterfacesAndSelfTo<CrosswordPersistence>().
            AsSingle().
            NonLazy();
    }
}
