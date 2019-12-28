using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorScript : MonoBehaviour
{
    float timer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > 1f)
        {
            timer = 0;
            SetCursor();
        }
    }

    private void SetCursor()
    {
        int startSceneID = 0;
        int sceneID = SceneManager.GetActiveScene().buildIndex;
        int scenesInBuild = SceneManager.sceneCountInBuildSettings;
        if (sceneID != startSceneID && sceneID != scenesInBuild && sceneID != scenesInBuild - 1)
        {
            Cursor.visible = false;
        }
        else
        {
            Cursor.visible = true;
        }
    }
}
