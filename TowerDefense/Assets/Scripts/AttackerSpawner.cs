using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackerSpawner : MonoBehaviour
{
    [SerializeField] private bool spawn = true;
    [SerializeField] Attacker lizard = null;
    [SerializeField] float minSpawnTime = 1f;
    [SerializeField] float maxSpawnTime = 5f;

    private Vector3 positionOffset = new Vector3(0.5f, 0.25f, 0f);
    private Vector2 spawnPosition;

    private void Start()
    {
        spawnPosition = transform.position + positionOffset;
        StartCoroutine(SpawnContinuously());
    }

    IEnumerator SpawnContinuously()
    {
        while (spawn)
        {
            yield return StartCoroutine(SpawnEnemy(UnityEngine.Random.Range(minSpawnTime, maxSpawnTime)));
        }
    }

    IEnumerator SpawnEnemy(float spawnTime)
    {
        yield return new WaitForSeconds(spawnTime);
        SpawnOneEnemy(lizard);
    }

    private void SpawnOneEnemy(Attacker enemyPrefab)
    {
        Attacker enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}
