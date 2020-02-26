using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class TimeScaleButton : MonoBehaviour
{
    [SerializeField] private Sprite[] arrows = new Sprite[5];
    [SerializeField] private int actualTimeScale = 1;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetActualTimeScaleArrow();
    }

    private void SetActualTimeScaleArrow()
    {
        actualTimeScale = Mathf.RoundToInt(Time.timeScale);
        spriteRenderer.sprite = arrows[actualTimeScale - 1];
    }

    private void OnMouseDown()
    {
        ChangeTimeScale();
    }

    private void ChangeTimeScale()
    {
        if (actualTimeScale >= arrows.Length) actualTimeScale = 0;
        actualTimeScale++;
        spriteRenderer.sprite = arrows[actualTimeScale - 1];
        Time.timeScale = actualTimeScale;
    }
}
