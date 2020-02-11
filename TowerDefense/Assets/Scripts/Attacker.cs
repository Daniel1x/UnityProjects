using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacker : MonoBehaviour
{
    [SerializeField] [Range(0f, 2f)] private float speed = 1f;
    [SerializeField] [Range(0f, 2f)] private float spawnTime = 1f;
    private bool isWalking = false;

    private void Start()
    {
        StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        yield return new WaitForSeconds(spawnTime);
        isWalking = true;
    }

    private void Update()
    {
        if (!isWalking) return;
        transform.Translate(Vector2.left * Time.deltaTime * speed);
    }

    public void SetMovementSpeed(float speed)
    {
        this.speed = speed;
    }
}
