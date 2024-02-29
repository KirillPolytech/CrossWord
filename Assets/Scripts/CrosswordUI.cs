using UnityEngine;

public class CrosswordUI : MonoBehaviour
{
    [SerializeField] private GameObject[] UI;

    private void Awake()
    {
        UI = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            UI[i] = transform.GetChild(i).gameObject;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            for (int i = 0; i < UI.Length; i++)
            {
                UI[i].SetActive(!UI[i].activeSelf);
            }
        }   
    }
}
