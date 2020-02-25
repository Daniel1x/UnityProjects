using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSession : MonoBehaviour
{
    [SerializeField] private int playerLives = 5;
    [SerializeField] private int score = 0;
    [SerializeField] private Text livesTextBox = null;
    [SerializeField] private Text scoreTextBox = null;
    public static GameSession activeGameSession = null;

    private void Awake()
    {
        SetUpSingleton();
    }

    private void Start()
    {
        activeGameSession = this;
        UpdateTexts();
    }

    private void SetUpSingleton()
    {
        int numOfExistingObjects = FindObjectsOfType(GetType()).Length;
        if (numOfExistingObjects > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public void HandlePlayerDeath()
    {
        if (playerLives > 1)
        {
            TakeLife();
        }
        else
        {
            ResetGameSession();
        }
        UpdateTexts();
    }

    private void TakeLife()
    {
        playerLives--;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    private void ResetGameSession()
    {
        SceneManager.LoadScene(0);
        Destroy(gameObject);
    }

    public void AddCoin(int howMany)
    {
        score += howMany;
        UpdateScoreText();
    }

    private void UpdateLivesText()
    {
        livesTextBox.text = playerLives.ToString();
    }

    private void UpdateScoreText()
    {
        scoreTextBox.text = score.ToString();
    }

    public void UpdateTexts()
    {
        UpdateLivesText();
        UpdateScoreText();
    }
}
