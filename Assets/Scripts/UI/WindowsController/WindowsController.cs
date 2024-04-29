using System.Linq;
using CrosswordWindows;
using UnityEngine;

public class WindowsController : MonoBehaviour
{
    [SerializeField] private Window[] windows;
    private void Start()
    {
        OpenWindow(windows[0].WindowName);
    }

    public void OpenWindow(string name)
    {
        foreach (var t in windows)
        {
            t.Close();
        }
        
        Window w = windows.FirstOrDefault(x => x.WindowName == name);
        if (w == false)
        {
            Debug.LogWarning($"Can't find window.");
        }
        else
        {
            w.Open();
        }
        
        Debug.Log($"Window changed: {name}");
    }
}
