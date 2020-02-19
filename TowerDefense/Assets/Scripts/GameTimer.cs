using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class GameTimer : MonoBehaviour
{
    [Tooltip("Time of level in seconds.")]
    [SerializeField] private float levelTime = 60f;
    private float timer = 0f;
    private Slider slider;
    private bool endOfTime = false;

    private void Start()
    {
        slider = GetComponent<Slider>();
    }

    private float CalculateSliderValue()
    {
        timer += Time.deltaTime;
        float sliderVal = Mathf.Clamp(timer / levelTime, 0f, 1f);
        return sliderVal;
    }

    private void Update()
    {
        if (endOfTime) return;
        float value = CalculateSliderValue();
        slider.value = value;
        if (value >= 1f)
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        endOfTime = true;
        FindObjectOfType<LevelController>().TimeOut();
    }
}
