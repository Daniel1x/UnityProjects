using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
    private AudioSource audioSource;

    private void SetUpSingleton()
    {
        int numberOfMusicPlayers = FindObjectsOfType(GetType()).Length;
        if (numberOfMusicPlayers > 1) Destroy(gameObject);
        else DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SetUpSingleton();
        PlayerPrefsController.SetDefaultVolume();
        PlayerPrefsController.SetDefaultDifficulty();
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = PlayerPrefsController.GetMasterVolume();
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }
}
