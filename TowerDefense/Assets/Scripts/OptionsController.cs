using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider = null;
    [SerializeField] private Slider difficultySlider = null;

    private MusicPlayer musicPlayer;

    private void Start()
    {
        volumeSlider.value = PlayerPrefsController.GetMasterVolume();
        difficultySlider.value = PlayerPrefsController.GetDifficulty();
        musicPlayer = FindObjectOfType<MusicPlayer>();
    }

    private void Update()
    {
        SetVolume();
    }

    private void SetVolume()
    {
        if (musicPlayer) musicPlayer.SetVolume(volumeSlider.value);        
    }

    public void SaveAndExit()
    {
        PlayerPrefsController.SetMasterVolume(volumeSlider.value);
        PlayerPrefsController.SetDifficulty(difficultySlider.value);
        FindObjectOfType<LevelLoader>().LoadMenuScene();
    }

    public void SetDefaults()
    {
        PlayerPrefsController.SetDefaultVolume();
        volumeSlider.value = PlayerPrefsController.GetMasterVolume();
        PlayerPrefsController.SetDefaultDifficulty();
        difficultySlider.value = PlayerPrefsController.GetDifficulty();
    }
}
