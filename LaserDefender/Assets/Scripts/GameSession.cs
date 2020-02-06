using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSession : MonoBehaviour
{
    [SerializeField] private int score = 0;
    public int Score { get => score; set => score = value; }

    private void Awake() => SetUpSingleton();

    private void SetUpSingleton()
    {
        int numberGameSessions = FindObjectsOfType(GetType()).Length;
        if (numberGameSessions > 1) Destroy(gameObject);
        else DontDestroyOnLoad(gameObject);
    }

    public void AddToScore(int scoreVal) => score += scoreVal;

    public void ResetGame() => Destroy(gameObject);
}
