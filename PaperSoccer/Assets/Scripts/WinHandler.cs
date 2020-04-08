using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class WinHandler : MonoBehaviour {

    [SerializeField] private Text winTextBox;
    [SerializeField] private Text statsTextBox;
    [SerializeField] private DataFile data;
    [SerializeField] private float delayTime = 1f;

    private GameManager gameManager;
    private MapCreator map;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameManager.OnPlayerWin += GameManager_OnPlayerWin;
        map = FindObjectOfType<MapCreator>();
        winTextBox.text = "";
        UpdateStats();
    }

    private void GameManager_OnPlayerWin(bool isPlayerOneTurn)
    {
        if (isPlayerOneTurn)
        {
            winTextBox.text = "Player One WIN!!!";
            data.PlayerOneWins++;
        }
        else
        {
            winTextBox.text = "Player Two WIN!!!";
            data.PlayerTwoWins++;
        }
        StartCoroutine(DelayReset());
    }

    public void DrawGame()
    {
        winTextBox.text = "Draw!";
        StartCoroutine(DelayReset());
    }

    public void WinGame()
    {
        winTextBox.text = "Player One WIN!!!";
        data.PlayerOneWins++;
        RestartGame();
    }

    public void LoseGame()
    {
        winTextBox.text = "Player Two WIN!!!";
        data.PlayerTwoWins++;
        RestartGame();
    }

    private IEnumerator DelayReset()
    {
        yield return new WaitForSeconds(delayTime);
        RestartGame();
        winTextBox.text = "";
    }

    public void RestartGame()
    {
        gameManager.RestartGameManager();
        map.RestartGame();
        UpdateStats();
    }

    private void UpdateStats()
    {
        statsTextBox.text = "Stats: \nP1: " + data.PlayerOneWins + ", P2: " + data.PlayerTwoWins;
    }

    public void ChangeMapSize(int size)
    {
        switch (size)
        {
            case 0:
                map.IncreaseXSize();
                break;
            case 1:
                map.IncreaseYSize();
                break;
            case 2:
                map.DecreaseXSize();
                break;
            case 3:
                map.DecreaseYSize();
                break;
        }
        RestartGame();
    }
}
