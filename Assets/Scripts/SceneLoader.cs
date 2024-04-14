using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public const string MainScene = "Crossword";
    public const string MenuScene = "Menu";

    public static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
