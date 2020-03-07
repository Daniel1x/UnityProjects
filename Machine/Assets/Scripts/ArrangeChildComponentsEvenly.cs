using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrangeChildComponentsEvenly : MonoBehaviour
{
    [SerializeField] private Vector3 offset = new Vector3();
    [SerializeField] private bool constantMove = false;

    private void Update()
    {
        if (constantMove || Input.GetKeyDown(KeyCode.Space)) 
        {
            ChangeChildsPosition();
        }
    }

    public void ChangeChildsPosition()
    {
        Transform[] childs = GetComponentsInChildren<Transform>();

        for(int i = 1; i < childs.Length; i++)
        {
            childs[i].position = transform.position + (i - 1) * offset;
        }
    }
}
