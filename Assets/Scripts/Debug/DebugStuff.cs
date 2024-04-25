using System.Collections;
using UnityEngine;

public class DebugStuff : MonoBehaviour
{
    private string _myLog = "";
    private string _output;
    private bool _isEnabled = true;
    public void Initialize()
    {
        Application.logMessageReceived += Log;
    }

    private void OnEnable()
    {
        StartCoroutine(StartLog());
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= Log;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            _isEnabled = !_isEnabled;
        }
    }
    
    private IEnumerator StartLog()
    {
        while (true)
        {
            yield return new WaitForSeconds(600);
            _myLog = "";
        }
    }

    private void Log(string logString, string stackTrace, LogType type)
    {
        _output = logString;
        _myLog = _output + " " + _myLog;
        if (_myLog.Length > 1000)
        {
            _myLog = _myLog[..700];
        }
    }

    private void OnGUI()
    {
        if (_isEnabled == false)
            return;
        
        GUI.TextArea(new Rect(Screen.width / 2, 10, Screen.width / 2, Screen.height / 4), _myLog);
    }
}