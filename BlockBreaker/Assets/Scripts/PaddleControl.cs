using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleControl : MonoBehaviour
{
    //configuration parameters
    [SerializeField] float minX = 1.0f;
    [SerializeField] float maxX = 15.0f;
    [SerializeField] float yOffset = 0.25f;
    [SerializeField] float screenSize = 16.0f;
    [SerializeField] GameObject ball;
    [SerializeField] float scaleFactor = 1f;
    [SerializeField] float minScale = 0.5f;
    [SerializeField] float maxScale = 8f;

    Vector2 mousePos = new Vector2();
    BallStart oneBall = null;
    BallStart[] balls = null;
    Level level = null;
    float minimalBallPosY = 0f;
    float timer = 0;

    private void Start()
    {
        mousePos.y = yOffset;
        oneBall = FindObjectOfType<BallStart>();
        level = FindObjectOfType<Level>();
        balls = FindObjectsOfType<BallStart>();
    }

    private void Update()
    {
        minX = transform.localScale.x;
        maxX = screenSize - transform.localScale.x;
        mousePos.x = Mathf.Clamp(GetXPosition(), minX, maxX);
        transform.position = mousePos;
        ChangeSizeX();
    }

    private float FindClosestBall()
    {
        UpdateBallsArray();
        float bestPosX = 0;
        minimalBallPosY = 999f;
        foreach (BallStart ball in balls)
        {
            if (ball != null)
            {
                if (ball.transform.position.y > 0 && ball.transform.position.y <= minimalBallPosY)
                {
                    minimalBallPosY = ball.transform.position.y;
                    bestPosX = ball.transform.position.x;
                }
            }
        }
        return bestPosX;
    }

    private void UpdateBallsArray()
    {
        timer += Time.deltaTime;
        if (timer > 1f)
        {
            balls = FindObjectsOfType<BallStart>();
            timer = 0;
        }
    }

    private float GetXPosition()
    {
        if (level.IsAutoPlayEnabled())
        {
            return FindClosestBall();
        }
        else
        {
            return screenSize * Input.mousePosition.x / Screen.width;
        }
    }

    private void ChangeSizeX()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            if (transform.localScale.x < maxScale)
            {
                transform.localScale += new Vector3(Time.deltaTime * scaleFactor, 0f, 0f);
            }
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (transform.localScale.x > minScale)
            {
                transform.localScale -= new Vector3(Time.deltaTime * scaleFactor, 0f, 0f);
            }
        }
    }
}
