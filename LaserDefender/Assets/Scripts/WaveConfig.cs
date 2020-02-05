using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Wave Config")]
public class WaveConfig : ScriptableObject
{
    [SerializeField] GameObject enemyPrefab = null;
    [SerializeField] GameObject pathPrefab = null;
    [SerializeField] float timeBetweenSpawns = 0.5f;
    [SerializeField] float spawnRandomFactor = 0.3f;
    [SerializeField] int numberOfEnemies = 5;
    [SerializeField] float enemySpeed = 2f;

    public GameObject EnemyPrefab => enemyPrefab;
    public GameObject PathPrefab => pathPrefab;
    public float TimeBetweenSpawns => timeBetweenSpawns;
    public float SpawnRandomFactor => spawnRandomFactor;
    public int NumberOfEnemies => numberOfEnemies;
    public float EnemySpeed => enemySpeed;
    public List<Transform> GetWaypoints()
    {
        List<Transform> waveWaypoints = new List<Transform>();
        foreach (Transform child in pathPrefab.transform)
        {
            waveWaypoints.Add(child);
        }
        return waveWaypoints;
    }
}
