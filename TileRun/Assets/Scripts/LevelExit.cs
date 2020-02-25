using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]
public class LevelExit : MonoBehaviour
{
    private PlayerMovement player = null;
    [SerializeField] [Range(0f, 5f)] private float levelLoadDelay = 1f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            player = collision.gameObject.GetComponent<PlayerMovement>();
            if (player && player.IsAlive)
            {
                OpenDoors();
            }
        }
    }

    private void OpenDoors()
    {
        GetComponent<Animator>().SetTrigger("openTrigger");
        StartCoroutine(LoadNextLevel());
    }

    private IEnumerator LoadNextLevel()
    {
        yield return new WaitForSeconds(levelLoadDelay);
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentSceneIndex >= SceneManager.sceneCountInBuildSettings - 1) currentSceneIndex = -1;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    private void CloseDoors()
    {
        GetComponent<Animator>().SetTrigger("closeTrigger");
    }

    private void Update()
    {
        if (!player) return;
        if (!player.IsAlive)
        {
            CloseDoors();
            player = null;
        }
    }
}
