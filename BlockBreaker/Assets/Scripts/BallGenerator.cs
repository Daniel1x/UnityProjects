using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallGenerator : MonoBehaviour
{
    [SerializeField] GameObject ballPrefab = null;

    float timeToNextGeneration = 0.1f;
    float timer = 0;

    private void GenerateOneBall()
    {
        GameObject ball = Instantiate(ballPrefab);
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            if (Input.GetKey(KeyCode.B))
            {
                GenerateOneBall();
                timer = timeToNextGeneration;
            }
        }
    }
}
