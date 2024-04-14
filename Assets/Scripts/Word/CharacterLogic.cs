using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CharacterLogic : MonoBehaviour
{   
    public readonly WordData WordData = new WordData();
    public bool IsCompleted { get; private set; }
    public bool IsSelected { get; private set; }

    private CrosswordUI _ui;
    private BoxCollider _boxCollider;
    
    public void Initialize(Vector3 horDir, Vector3 vertDir, Vector3 dir)
    {
        _ui = FindAnyObjectByType<CrosswordUI>();
        _boxCollider = GetComponent<BoxCollider>();
        
        float ind = (float)WordData.Characters.Length / 2;
        float remain = ind % 1 == 0 ? 0.5f : 0;
        
        if (dir == horDir)
        {
            _boxCollider.size = new Vector3(Mathf.Abs(horDir.x * WordData.Characters.Length),1,1);
            float xc = WordData.Characters[(int)ind].transform.position.x ;
            _boxCollider.center = new Vector3( xc - remain * -Mathf.Sign(xc), transform.GetChild(0).transform.position.y, 0 );
        }
        else
        {
            _boxCollider.size = new Vector3(1,Mathf.Abs(vertDir.y * WordData.Characters.Length),1);
            float yc = WordData.Characters[(int)ind].transform.position.y;
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

        bool isOpened = _ui.OpenInputPanel(this);

        IsSelected = isOpened;

        if (isOpened == true)
            ChangeWordColor(ColorType.selected);
    }

    public void ChangeWordColor(ColorType color)
    {
        if (IsCompleted == true) 
            return;

        Color col = color switch
        {
            ColorType.selected => Color.yellow,
            ColorType.finished => Color.green,
            ColorType.normal => Color.white,
            ColorType.highlighted => Color.blue,
            _ => Color.white
        };

        foreach (var c in WordData.Characters)
        {
            c.MeshRenderer.material.color = col;
        }
    }

    public void CheckWordCompletion()
    {
        if (IsCompleted == true)
            return;

        foreach (var item in WordData.Characters)
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

        foreach (var item in WordData.Characters)
        {
            if (item == null)
                continue;
            
            item.CurrentChar.color = Color.green;
        }

        IsCompleted = true;

        _ui.CloseInputPanel();

        ChangeWordColor(ColorType.finished);

        Debug.Log("Completed");
    }
}