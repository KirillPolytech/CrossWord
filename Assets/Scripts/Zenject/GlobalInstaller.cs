using UnityEngine;
using Zenject;

public class GlobalInstaller : MonoInstaller
{
    [SerializeField] private CrosswordFilesStorage crosswordFilesStorage;
    public override void InstallBindings()
    {
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
            FromNewComponentOnNewGameObject().
            AsSingle().NonLazy();
    }

    private void BindGamePreference()
    {
        Container.
            Bind<GamePreference>().
            FromNew().
            AsSingle();
    }

    private void BindCrosswordFilesStorage()
    {
        Container.
            Bind<CrosswordFilesStorage>().
            FromInstance(crosswordFilesStorage).
            AsSingle().NonLazy();
    }
    
    private void BindCustomCrosswordsStorage()
    {
        Container.
            Bind<CrosswordPersistence>().
            FromNew().
            AsSingle();
    }
}
