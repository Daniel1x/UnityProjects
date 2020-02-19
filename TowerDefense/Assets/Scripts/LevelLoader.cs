using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    private int currentSceneIndex;
    [SerializeField] private float timeToWait = 10f;

    private void Start()
    {
        //SetUpSingleton();
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentSceneIndex == 0) StartCoroutine(WaitForAWhile());
    }

    private void SetUpSingleton()
    {
        int numberOfLevelLoaders = FindObjectsOfType(GetType()).Length;
        if (numberOfLevelLoaders > 1) Destroy(gameObject);
        else DontDestroyOnLoad(gameObject);
    }

    private IEnumerator WaitForAWhile()
    {
        yield return new WaitForSeconds(timeToWait);
        LoadNextScene();
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    public void ReloadThisLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void LoadMenuScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartScreen");
    }

    public void LoadOptionsScene()
    {
        SceneManager.LoadScene("OptionsScreen");
    }

    public void LoadFirstLevel()
    {
        SceneManager.LoadScene("Level 1");
    }

    public void LoadNextSceneWithDelay(float delayInSeconds)
    {
        StartCoroutine(DelayLoading(delayInSeconds));
    }

    private IEnumerator DelayLoading(float delayInSeconds)
    {
        yield return new WaitForSeconds(delayInSeconds);
        LoadNextScene();
    }

    public void LoadGameOverScene()
    {
        SceneManager.LoadScene("GameOverScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
