using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

    Level level = null;

    private void Start()
    {
        level = FindObjectOfType<Level>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LoadStartScene();
        }
    }

    public void LoadNextScene()
    {
        if (level != null)
        {
            level.ClearCounting();
        }
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    public void LoadStartScene()
    {
        if (level != null)
        {
            level.ClearCounting();
            level.ClearPoints();
        }
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        if (level != null)
        {
            level.ClearCounting();
        }
        Application.Quit();
    }
}
