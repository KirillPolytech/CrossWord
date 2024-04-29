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
            Bind<InputHandler>().
            FromInstance(inputHandler).
            //FromNewComponentOnNewGameObject().
            AsSingle().
            NonLazy();
    }

    private void BindGamePreference()
    {
        Container.
            Bind<GamePreference>().
            FromNew().
            AsSingle().
            NonLazy();;
    }

    private void BindCrosswordFilesStorage()
    {
        Container.
            Bind<CrosswordFilesStorage>().
            FromInstance(crosswordFilesStorage).
            AsSingle().
            NonLazy();
    }
    
    private void BindCustomCrosswordsStorage()
    {
        Container.
            BindInterfacesAndSelfTo<CrosswordPersistence>().
            FromNew().
            AsSingle().
            NonLazy();
    }
}
