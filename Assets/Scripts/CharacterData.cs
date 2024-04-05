using TMPro;
using UnityEngine;

public class CharacterData : MonoBehaviour
{
    public CharacterLogic characterLogic;
    public MeshRenderer meshRenderer;
    public TextMeshProUGUI text;
    public Color color;
    public int charIndex;
    public char desiredChar;

    public void Initialize(Color c, int ind, char desiredchar, CharacterLogic logic) 
    {
        text.text = " ";//desiredchar.ToString();

        desiredChar = desiredchar;
        color = c;
        charIndex = ind;
        characterLogic = logic;
    }

    public void OpenPanel()
    {
        characterLogic.OpenMenu();
    }   
}