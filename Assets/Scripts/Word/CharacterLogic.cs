using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CharacterLogic : MonoBehaviour
{   
    public WordData wordData = new WordData();
    public string wordDescription;
    public bool IsCompleted { get; private set; }

    private CrosswordUI UI;
    private BoxCollider _boxCollider;

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }
    
    public void Initialize(Vector3 horDir, Vector3 vertDir, Vector3 dir)
    {
        UI = FindAnyObjectByType<CrosswordUI>();
        _boxCollider = GetComponent<BoxCollider>();
        
        float ind = (float)wordData.characters.Length / 2;
        float remain = ind % 1 == 0 ? 0.5f : 0;
        
        if (dir == horDir)
        {
            _boxCollider.size = new Vector3(Mathf.Abs(horDir.x * wordData.characters.Length),1,1);
            float xc = wordData.characters[(int)ind].transform.position.x ;
            _boxCollider.center = new Vector3( xc - remain * -Mathf.Sign(xc), transform.GetChild(0).transform.position.y, 0 );
        }
        else
        {
            _boxCollider.size = new Vector3(1,Mathf.Abs(vertDir.y * wordData.characters.Length),1);
            float yc = wordData.characters[(int)ind].transform.position.y;
            _boxCollider.center = new Vector3(transform.GetChild(0).transform.position.x, yc - remain * -Mathf.Sign(yc), 0 );
        }
    }

    private void Start()
    {
        IsCompleted = false;
    }

    public void OpenMenu()
    {
        if (IsCompleted == true)
            return;

        bool isOpened = UI.OpenInputPanel(this);

        if (isOpened == true)
            ChangeWordColor(ColorType.selected);
    }

    public void ChangeWordColor(ColorType color)
    {
        if (IsCompleted == true) 
            return;

        Color col;
        switch (color)
        {
            case ColorType.selected:
                col = Color.yellow;
                break;
            case ColorType.finished:
                col = Color.green;
                break;
            case ColorType.normal:
                col = Color.white;
                break;
            default: 
                col = Color.white;
                break;
        }

        foreach (var c in wordData.characters)
        {
            c.MeshRenderer.material.color = col;
        }
    }

    public void CheckWordCompletion()
    {
        if (IsCompleted == true)
            return;

        foreach (var item in wordData.characters)
        {
            if (item == null)
                continue;

            string str = item.DesiredChar.ToString().Trim();
            string str2 = item.CurrentChar.text.Trim();

            //Debug.Log($"InputField: {str2} DesiredChar: {str}");

            if (str2 != str)
            {
                return;
            }                
        }

        foreach (var item in wordData.characters)
        {
            if (item == null)
                continue;
            
            item.CurrentChar.color = Color.green;
        }

        IsCompleted = true;

        UI.CloseInputPanel();

        ChangeWordColor(ColorType.finished);

        Debug.Log("Completed");
    }
}