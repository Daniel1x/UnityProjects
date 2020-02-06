using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    private Text scoreTextBox;
    private GameSession gameSession;

    private void Start()
    {
        scoreTextBox = GetComponent<Text>();
        gameSession = FindObjectOfType<GameSession>();
    }

    private void Update()
    {
        scoreTextBox.text = gameSession.Score.ToString();
    }
}
