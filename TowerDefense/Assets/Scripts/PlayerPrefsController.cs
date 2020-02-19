using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsController : MonoBehaviour
{
    private const float MIN_VOLUME = 0f;
    private const float MAX_VOLUME = 1f;
    private const float DEFAULT_VOLUME = 0.5f;
    private const string VOLUME_KEY = "master volume";
    
    private const float MIN_DIFFICULTY = 0f;
    private const float MAX_DIFFICULTY = 1f;
    private const string DIFFICULTY_KEY = "difficulty";
    private const float DEFAULT_DIFFICULTY = 0f;

    public static void SetMasterVolume(float volume)
    {
        if (volume < MIN_VOLUME || volume > MAX_VOLUME)
        {
            Debug.Log("Volume is out of range.");
        }
        else PlayerPrefs.SetFloat(VOLUME_KEY, volume);
    }

    public static void SetDifficulty(float difficulty)
    {
        if (difficulty < MIN_DIFFICULTY || difficulty > MAX_DIFFICULTY)
        {
            Debug.Log("Difficulty is out of range.");
        }
        else PlayerPrefs.SetFloat(DIFFICULTY_KEY, difficulty);
    }

    public static float GetMasterVolume()
    {
        return PlayerPrefs.GetFloat(VOLUME_KEY);
    }

    public static void SetDefaultVolume()
    {
        PlayerPrefs.SetFloat(VOLUME_KEY, DEFAULT_VOLUME);
    }

    public static float GetDifficulty()
    {
        return PlayerPrefs.GetFloat(DIFFICULTY_KEY);
    }

    public static void SetDefaultDifficulty()
    {
        PlayerPrefs.SetFloat(DIFFICULTY_KEY, DEFAULT_DIFFICULTY);
    }

    public static void SetDefaults()
    {
        SetDefaultVolume();
        SetDefaultDifficulty();
    }
}
