using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class AudioScript : MonoBehaviour
{
    public AudioClip Music;
    public AudioSource MusicSource;
    public Text Timer;
    private float time = 0;
    private int sec = 0;
    private int min = 0;
    private int hour = 0;
    
    void Start()
    {
        MusicSource.clip = Music;
        MusicSource.Play();
    }

    private void Update()
    {
        CountTime();
        ConvertToTime();
        PrintTime();
    }

    private void CountTime()
    {
        time += Time.deltaTime;
    }

    private void ConvertToTime()
    {
        if (time >= 1)
        {
            sec++;
            time--;
        }
        if (sec >= 60)
        {
            min++;
            sec = 0;
        }
        if (min >= 60)
        {
            hour++;
            min = 0;
        }
    }

    private void PrintTime()
    {
        Timer.text = hour.ToString() + ":" + min.ToString() + ":" + sec.ToString();
    }
}
