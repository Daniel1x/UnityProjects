using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackerSpawner : MonoBehaviour
{
    private bool spawn = true;
    public bool Spawn { set => spawn = value; }
    private Vector2 spawnPosition;
    private float[] tresholdArray = null;
    private GameTimer gameTimer = null;
    public static float maxSpawnTimeOfAttackers = 5f;

    [Serializable]
    public class AttackerOptions
    {
        public Attacker attacker;
        [Range(0f, 1f)] public float spawnProbability = 0f;
        [Range(0.5f, 30f)] public float minSpawnTime = 0.5f;
        [Range(0.5f, 30f)] public float maxSpawnTime = 0.5f;
        public float timeDifference = 0f;
        public float probabilityTreshold = 0f;
        public void NormalizeProbability(float sumOfProbabilities)
        {
            spawnProbability = spawnProbability / sumOfProbabilities;
        }
    }
    [SerializeField] private AttackerOptions[] attackersArray = null;

    private void SetUpDifficulty(float difficulty, AttackerOptions attacker)
    {
        attacker.minSpawnTime -= attacker.minSpawnTime * difficulty;
        attacker.maxSpawnTime -= attacker.maxSpawnTime * difficulty;
        attacker.minSpawnTime = Mathf.Clamp(attacker.minSpawnTime, 0.5f, attacker.maxSpawnTime);
        attacker.maxSpawnTime = Mathf.Clamp(attacker.maxSpawnTime, attacker.minSpawnTime, 30f);
        attacker.timeDifference = attacker.maxSpawnTime - attacker.minSpawnTime;
    }

    private void SetUpAttackersDifficulty()
    {
        float difficulty = PlayerPrefsController.GetDifficulty();
        for (int i = 0; i < attackersArray.Length; i++)
            SetUpDifficulty(difficulty, attackersArray[i]);
    }

    private void NormalizeAttackersSpawnProbability()
    {
        float longestSpawnTime = 0;
        float sumOfProbabilities = 0;
        float treshold = 0;
        tresholdArray = new float[attackersArray.Length];

        for (int i = 0; i < attackersArray.Length; i++)
            sumOfProbabilities += attackersArray[i].spawnProbability;

        for (int i = 0; i < attackersArray.Length; i++)
        {
            AttackerOptions attacker = attackersArray[i];
            attacker.NormalizeProbability(sumOfProbabilities);
            treshold += attacker.spawnProbability;
            attacker.probabilityTreshold = treshold;
            tresholdArray[i] = treshold;
            if (attacker.minSpawnTime > longestSpawnTime)
                longestSpawnTime = attacker.minSpawnTime;
        }
        maxSpawnTimeOfAttackers = longestSpawnTime;
    }

    private void Start()
    {
        spawnPosition = transform.position;
        gameTimer = FindObjectOfType<GameTimer>();
        SetUpAttackersDifficulty();
        NormalizeAttackersSpawnProbability();
        StartCoroutine(SpawnContinuously());
    }

    IEnumerator SpawnContinuously()
    {
        while (spawn)
        {
            yield return StartCoroutine(SpawnEnemy());
        }
    }

    IEnumerator SpawnEnemy()
    {
        int attackerID = ChooseRandomAttacker();
        float spawnTime = ChooseRandomSpawnTimeForAttacker(attackerID);
        yield return new WaitForSeconds(spawnTime);
        SpawnOneEnemy(attackersArray[attackerID].attacker);
    }

    private float ChooseRandomSpawnTimeForAttacker(int attackerID)
    {
        return UnityEngine.Random.Range(attackersArray[attackerID].minSpawnTime, 
                                        attackersArray[attackerID].maxSpawnTime);
    }

    private int ChooseRandomAttacker()
    {
        float randomValue = UnityEngine.Random.Range(0f, 1f);
        for (int i = 0; i < tresholdArray.Length; i++)
            if (tresholdArray[i] >= randomValue)
                return i;

        Debug.Log("Attacker ID not found!");
        return -1;
    }

    private void SpawnOneEnemy(Attacker enemyPrefab)
    {
        Attacker enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, transform);
    }

    private void Update()
    {
        UpdateSpawnTime();
    }

    private void UpdateSpawnTime()
    {
        for(int i = 0; i < attackersArray.Length; i++)
        {
            AttackerOptions attacker = attackersArray[i];
            attacker.maxSpawnTime = attacker.minSpawnTime + ((1 - gameTimer.PercentageTimeLevel) * attacker.timeDifference);
        }
    }
}
