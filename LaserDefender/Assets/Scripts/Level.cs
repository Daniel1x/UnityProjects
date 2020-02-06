using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    [SerializeField] private float dyingTime = 3f;

    public void LoadStartMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadMainGame()
    {
        SceneManager.LoadScene("Main");
        GameSession gs = FindObjectOfType<GameSession>();
        if (gs != null) gs.ResetGame();
    }

    public void LoadGameOver()
    {
        SceneManager.LoadScene("GameOver");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private IEnumerator DelayGameOver()
    {
        yield return new WaitForSeconds(dyingTime);
        LoadGameOver();
    }

    public void LoadDelayedGameOver()
    {
        StartCoroutine(DelayGameOver());
    }
}
