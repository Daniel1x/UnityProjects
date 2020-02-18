using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackerSpawner : MonoBehaviour
{
    [SerializeField] private bool spawn = true;
    [SerializeField] Attacker[] attackers = null;
    [SerializeField] float minSpawnTime = 1f;
    [SerializeField] float maxSpawnTime = 5f;
    
    private Vector2 spawnPosition;

    private void Start()
    {
        spawnPosition = transform.position;
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
        SpawnOneEnemy(attackers[UnityEngine.Random.Range(0, attackers.Length)]);
    }

    private void SpawnOneEnemy(Attacker enemyPrefab)
    {
        Attacker enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, transform);
    }
}
