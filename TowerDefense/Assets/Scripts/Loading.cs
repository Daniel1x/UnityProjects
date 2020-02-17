using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class Loading : MonoBehaviour
{
    private Text text;
    private float time;
    private int maxDots = 7;
    private int actualDots = -1;
    [SerializeField] [Range(0.1f, 1f)] private float timeInterval = 0.25f;
    private string loading = "LOADING";

    private void Start()
    {
        text = GetComponent<Text>();
    }

    private void Update()
    {
        time += Time.deltaTime;
        if (time > timeInterval) LoadNextDot();
    }

    private void LoadNextDot()
    {
        time = 0;
        actualDots++;
        if (actualDots > maxDots) actualDots = 0;
        string newText = loading;
        for (int i = 0; i < actualDots; i++) newText += ".";
        text.text = newText;
    }
}
