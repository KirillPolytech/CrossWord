using TMPro;
using UnityEngine;

public class CharacterData : MonoBehaviour
{
    public CharacterLogic characterLogic;
    public MeshRenderer meshRenderer;
    public TextMeshProUGUI currentChar;
    public Color color;
    public int charIndex;
    public char desiredChar;

    public void OpenPanel()
    {
        characterLogic.OpenMenu();
    }   
}