using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class PulsingText : MonoBehaviour
{
    private Text text;
    private int baseFontSize;
    [SerializeField] private int amplitudeOfFontSizeChange = 5;
    private float time;

    private void Start()
    {
        text = GetComponent<Text>();
        baseFontSize = text.fontSize;
    }

    private void Update() => UpdateFontSize();

    private void FixedUpdate() => UpdateFontSize();

    private void UpdateFontSize()
    {
        time += Time.deltaTime;
        text.fontSize = baseFontSize + (int)(amplitudeOfFontSizeChange * Mathf.Sin(Mathf.PI * time));
    }
}
