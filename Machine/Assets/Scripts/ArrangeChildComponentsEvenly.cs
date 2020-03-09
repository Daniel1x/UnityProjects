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

        for(int childIndex = 1; childIndex < childs.Length; childIndex++)
        {
            childs[childIndex].position = transform.position + (childIndex - 1) * offset;
        }
    }
}
