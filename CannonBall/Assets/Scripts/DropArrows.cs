using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropArrows : MonoBehaviour
{
    public GameObject arrow;
    public int arrows;
    [Range(0f,100f)]public float range=20f;

    private void Start()
    {
        for(int i = 0; i < arrows; i++)
        {
            Instantiate(arrow, 5f * Vector3.up + Quaternion.Euler(0f, Random.Range(0f, 360f), 0f) * Vector3.forward * Random.Range(0f,range), 
                        Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f)));
        }
    }
}
