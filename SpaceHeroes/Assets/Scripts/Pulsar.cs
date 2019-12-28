using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulsar : MonoBehaviour
{
    private float time = 0.0f;
    private bool BigToSmall = true;
    private float Scale = 1.0f;
    private float MaxScale = 2f;
    public float ScaleChange = 0.1f;
    public float PulsarFrequency = 1.0f;
    private Vector3 ScaleV = new Vector3();

    private void Start()
    {
        ScaleV = gameObject.transform.localScale;
        Scale = ScaleV.x;
        MaxScale = ScaleChange*Scale;
    }

    private void Update()
    {
        CountTime();
        ChangeSize();
    }

    private void CountTime()
    {
        time += Time.deltaTime;
        if (time >= 1/PulsarFrequency)
        {
            BigToSmall = !BigToSmall;
            time = 0.0f;
        }
    }

    private void ChangeSize()
    {
        if (BigToSmall)
        {
            Scale += (MaxScale * Time.deltaTime) * PulsarFrequency;
        }
        else
        {
            Scale -= (MaxScale * Time.deltaTime) * PulsarFrequency;
        }
        ScaleV = new Vector3(Scale, Scale, Scale);
        gameObject.transform.localScale = ScaleV;
    }
}
