using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] List<WaveConfig> waveConfigs = new List<WaveConfig>();
    [SerializeField] float timeToNextWave = 5f;
    [SerializeField] bool looping = false;
    [SerializeField] float spawnHeightOffset = 1f;

    private IEnumerator Start()
    {
        do
        {
            yield return StartCoroutine(SpawnAllWaves());
        }
        while (looping);
    }

    private IEnumerator SpawnAllEnemiesInWave(WaveConfig currentWave)
    {
        for(int enemyID = 0; enemyID < currentWave.NumberOfEnemies; enemyID++)
        {
            GameObject enemy = Instantiate(currentWave.EnemyPrefab, 
                currentWave.GetWaypoints()[0].transform.position + spawnHeightOffset * Vector3.up, Quaternion.identity);

            enemy.GetComponent<EnemyPathing>().SetWaveConfig(currentWave);
            yield return new WaitForSeconds(currentWave.TimeBetweenSpawns);
        }
        yield return null;
    }

    private IEnumerator SpawnAllWaves()
    {
        for(int waveID = 0; waveID < waveConfigs.Count; waveID++)
        {
            yield return StartCoroutine(SpawnAllEnemiesInWave(waveConfigs[waveID]));
            yield return new WaitForSeconds(timeToNextWave);
        }
    }
}
