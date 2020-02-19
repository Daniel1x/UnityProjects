using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackerSpawner : MonoBehaviour
{
    private bool spawn = true;
    public bool Spawn { set => spawn = value; }
    [SerializeField] private Attacker[] attackers = null;
    [SerializeField] private float minSpawnTime = 1f;
    [SerializeField] private float maxSpawnTime = 5f;
    public float MaxSpawnTime { get => maxSpawnTime; }
    private Vector2 spawnPosition;

    private void Start()
    {
        SetUpDifficulty();
        spawnPosition = transform.position;
        StartCoroutine(SpawnContinuously());
    }

    private void SetUpDifficulty()
    {
        float difficulty = PlayerPrefsController.GetDifficulty();
        minSpawnTime -= minSpawnTime * difficulty;
        maxSpawnTime -= maxSpawnTime * difficulty;
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
