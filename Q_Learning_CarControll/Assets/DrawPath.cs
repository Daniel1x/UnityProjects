using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPath : MonoBehaviour
{
    Vector3 oldPos;
    public Color color;
    public float multiplier = 1f;
    public float drawTime = 20f;
    public int frameInterval = 2;
    int frame = 0;
    public bool drawPath;
    public bool drawForward;

    private void Start()
    {
        oldPos = transform.position;
    }
    private void Update()
    {
        if(drawPath)
            Debug.DrawLine(oldPos, transform.position, color, drawTime);
        oldPos = transform.position;

        frame++;
        if(drawForward && frame%frameInterval==0)
            Debug.DrawLine(transform.position, transform.position + multiplier * transform.forward, color, drawTime);
    }
}
