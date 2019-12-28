using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TextScript : MonoBehaviour
{
    public Text gravityMode;
    private bool Mode = true;
    
    void Start()
    {
        ModeChange();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            Mode = !Mode;
            ModeChange();
        }
    }

    private void ModeChange()
    {
        if (Mode)
        {
            gravityMode.text = "PULL";
        }
        else
        {
            gravityMode.text = "PUSH";
        }
    }
    
}
