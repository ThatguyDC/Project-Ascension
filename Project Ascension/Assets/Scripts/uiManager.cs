using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class uiManager : MonoBehaviour
{
    [Header("Mechanics UI")]
    private GameObject Interact;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowUI(GameObject uiElement) //takes game obj as arg to display 
    {
        uiElement.SetActive(true);  
    }

    public void HideUI(GameObject uiElement)
    {
        uiElement.SetActive(false);
    }
}
