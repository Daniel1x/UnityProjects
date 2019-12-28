using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseCollider : MonoBehaviour
{
    Level level = null;

    private void Start()
    {
        level = FindObjectOfType<Level>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        level.SubtractOneBall();
        if (level.playableBalls <= 0)
        {
            SceneManager.LoadScene("Game Over");
        }
    }
}
