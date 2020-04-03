using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinHandler : MonoBehaviour {

    [SerializeField] private Text WinTextBox;
    [SerializeField] private DataFile data;
    [SerializeField] private float delayTime = 1f;

    private GameManager gameManager;
    private MapCreator map;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameManager.OnPlayerWin += GameManager_OnPlayerWin;
        map = FindObjectOfType<MapCreator>();
        WinTextBox.text = "";
    }

    private void GameManager_OnPlayerWin(bool isPlayerOneTurn)
    {
        if (isPlayerOneTurn)
        {
            WinTextBox.text = "Player One WIN!!!";
            data.PlayerOneWins++;
        }
        else
        {
            WinTextBox.text = "Player Two WIN!!!";
            data.PlayerTwoWins++;
        }
        StartCoroutine(DelayReset());
    }

    public void DrawGame()
    {
        WinTextBox.text = "Draw!";
        StartCoroutine(DelayReset());
    }

    private IEnumerator DelayReset()
    {
        yield return new WaitForSeconds(delayTime);
        RestartGame();
    }

    public void RestartGame()
    {
        gameManager.RestartGameManager();
        map.RestartGame();
    }
}
