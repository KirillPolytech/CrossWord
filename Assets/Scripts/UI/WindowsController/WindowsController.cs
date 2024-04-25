using CrosswordWindows;
using UnityEngine;

public class WindowsController : MonoBehaviour
{
    [SerializeField] private Window[] windows;

    public DefaultWindow DefaultWindow;
    public CrosswordUI GameUI;
    public InGameMenu InGameWindow;
    public InputWindow InputFieldWindow;
    private void Start()
    {
        OpenWindow(GameUI);
    }

    public void OpenWindow(Window window)
    {
        foreach (var t in windows)
        {
            t.Close();
        }
        
        window.Open();
    }
}
