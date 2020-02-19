using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LivesDisplay : MonoBehaviour
{
    [SerializeField] private int lives = 10;
    [SerializeField] private Animator heartAnimator = null;
    private Text livesBox;
    private float startLives;

    private void Start()
    {
        SetUpDifficulty();
        startLives = lives;
        livesBox = GetComponent<Text>();
        UpdateLivesDisplay();
    }

    private void SetUpDifficulty()
    {
        float difficulty = PlayerPrefsController.GetDifficulty();
        lives -= (int)(difficulty * lives) - 10;
    }

    private void UpdateLivesDisplay()
    {
        if (lives < 0)
        {
            lives = 0;
            FindObjectOfType<LevelController>().LoseGame();
        }
        livesBox.text = lives.ToString();
        heartAnimator.SetFloat("HeartBitSpeed", HeartSpeed());
    }

    private float HeartSpeed()
    {
        if (lives <= 0) return 0f;
        float speed = 1 + 10 * (1 - lives / startLives);
        return speed;
    }

    public void AddLives(int amount)
    {
        lives += amount;
        UpdateLivesDisplay();
    }

    public void TakeLifes(int amount = 1)
    {
        lives -= amount;
        UpdateLivesDisplay();
    }
}
