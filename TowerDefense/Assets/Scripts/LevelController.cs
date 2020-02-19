using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LevelController : MonoBehaviour
{
    [SerializeField] GameObject winCanvas = null;
    [SerializeField] GameObject loseCanvas = null;
    [SerializeField] private float endGameDelay = 5f;
    [SerializeField] private float loadNextSceneDelay = 5f;
    private AttackerSpawner[] attackerSpawners;
    private int numberOfAttackers = 0;
    private bool levelTimerFinished = false;
    private bool gameFinished = false;

    private void Start()
    {
        if (winCanvas != null) winCanvas.SetActive(false);
        if (loseCanvas != null) loseCanvas.SetActive(false);
        attackerSpawners = FindObjectsOfType<AttackerSpawner>();
    }

    private void Update()
    {
        if (gameFinished) return;
        numberOfAttackers = CountEnemies();
        CheckForGameFinish();
    }

    private int CountEnemies()
    {
        int numEnemies = 0;
        for(int i = 0; i < attackerSpawners.Length; i++)
        {
            numEnemies += attackerSpawners[i].transform.childCount;
        }
        return numEnemies;
    }

    public void TimeOut()
    {
        levelTimerFinished = true;
        BlockAttackersSpawn();
    }

    private void BlockAttackersSpawn()
    {
        for(int i = 0; i < attackerSpawners.Length; i++)
        {
            attackerSpawners[i].Spawn = false;
        }
    }

    public void CheckForGameFinish()
    {
        if (numberOfAttackers <= 0 && levelTimerFinished) StartCoroutine(WaitForSpawners());        
    }

    IEnumerator WaitForSpawners()
    {
        endGameDelay = attackerSpawners[0].MaxSpawnTime;
        yield return new WaitForSeconds(endGameDelay);
        if (numberOfAttackers <= 0 && levelTimerFinished)
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        gameFinished = true;
        if (winCanvas != null) winCanvas.SetActive(true);
        GetComponent<AudioSource>().Play();
        FindObjectOfType<LevelLoader>().LoadNextSceneWithDelay(loadNextSceneDelay);
    }

    public void LoseGame()
    {
        if (loseCanvas != null) loseCanvas.SetActive(true);
        Time.timeScale = 0f;
    }
}
