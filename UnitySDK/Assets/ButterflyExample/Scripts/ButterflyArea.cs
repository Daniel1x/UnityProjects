using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using TMPro;
using System;

public class ButterflyArea : Area
{
    public ButterflyAgent butterflyAgent;
    public GameObject targetPrefab;
    public TextMeshPro cumulativeRewardText;

    public float targetSpeed = 0f;
    public float catchRange = 1f;
    public float butterflySpawnRange = 5f;
    public float targetSpawnRange = 10f;
    public int numTargetsToSpawn = 5;
    public bool spawnStraight = false;

    public List<GameObject> targetList;

    public override void ResetArea()
    {
        RemoveAllTargets();
        SpawnButterfly();
        SpawnTargets(numTargetsToSpawn, targetSpeed, true, UnityEngine.Random.Range(0f, 360f), spawnStraight);
    }

    public void RemoveSpecificTarget(GameObject targetObject)
    {
        targetList.Remove(targetObject);
        Destroy(targetObject);
    }

    public static Vector3 ChooseRandomPosition(Vector3 center, float minAngleDeg, float maxAngleDeg, float minRadius, float maxRadius, bool spawnInOnePlane = true)
    {
        float radius = (maxRadius < minRadius) ? minRadius : UnityEngine.Random.Range(minRadius, maxRadius);
        Quaternion angle = new Quaternion();
        if (spawnInOnePlane)
        {
            angle = Quaternion.Euler(0f, UnityEngine.Random.Range(minAngleDeg, maxAngleDeg), 0f);
        }
        else
        {
            angle = Quaternion.Euler(UnityEngine.Random.Range(minAngleDeg, maxAngleDeg), UnityEngine.Random.Range(minAngleDeg, maxAngleDeg), UnityEngine.Random.Range(minAngleDeg, maxAngleDeg));
        }
        Vector3 position = center + (angle * (Vector3.forward * radius));
        return position;
    }

    private void RemoveAllTargets()
    {
        if (targetList == null) return;

        for (int i = 0; i < targetList.Count; i++)
        {
            if (targetList[i] != null) Destroy(targetList[i]);
        }
        targetList = new List<GameObject>();
    }

    private void SpawnButterfly()
    {
        butterflyAgent.transform.position = ChooseRandomPosition(transform.position, 0f, 360f, 0f, butterflySpawnRange) + Vector3.up;
        butterflyAgent.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360), 0f);
    }

    private void SpawnTargets(int numTargetsToSpawn, float targetSpeed, bool inCircle = false, float bonusAngle = 0f, bool spawnStraight = false)
    {
        if (!spawnStraight)
        {
            if (!inCircle)
            {
                for (int i = 0; i < numTargetsToSpawn; i++)
                {
                    GameObject targetObject = Instantiate(targetPrefab);
                    targetObject.transform.position = ChooseRandomPosition(transform.position, 0f, 360f, butterflySpawnRange + 1f, targetSpawnRange) + Vector3.up;
                    targetObject.transform.rotation = Quaternion.Euler(UnityEngine.Random.Range(0f, 360), UnityEngine.Random.Range(0f, 360), UnityEngine.Random.Range(0f, 360));
                    targetObject.transform.parent = transform;
                    targetList.Add(targetObject);
                    targetObject.GetComponent<Target>().targetSpeed = targetSpeed;
                }
            }
            else
            {
                for (int i = 0; i < numTargetsToSpawn; i++)
                {
                    GameObject targetObject = Instantiate(targetPrefab);
                    targetObject.transform.position = transform.position + ((-10f + (i * 20 / numTargetsToSpawn)) * Vector3.up)
                                                    + (Quaternion.Euler(0f, bonusAngle + (i * 360f / numTargetsToSpawn), 0f) * (Vector3.forward * targetSpawnRange));
                    targetObject.transform.rotation = Quaternion.Euler(UnityEngine.Random.Range(0f, 360), UnityEngine.Random.Range(0f, 360), UnityEngine.Random.Range(0f, 360));
                    targetObject.transform.parent = transform;
                    targetList.Add(targetObject);
                    targetObject.GetComponent<Target>().targetSpeed = targetSpeed;
                }
            }
        }
        else
        {
            for(int i = 0; i < numTargetsToSpawn; i++)
            {
                GameObject targetObject = Instantiate(targetPrefab);

                targetObject.transform.position = transform.position + (transform.rotation * (Vector3.forward * (10f + (i * 450f / numTargetsToSpawn))));

                targetObject.transform.rotation = Quaternion.Euler(UnityEngine.Random.Range(0f, 360), UnityEngine.Random.Range(0f, 360), UnityEngine.Random.Range(0f, 360));
                targetObject.transform.parent = transform;
                targetList.Add(targetObject);
                targetObject.GetComponent<Target>().targetSpeed = 0f;
            }
        }
    }

    private void Update()
    {
        cumulativeRewardText.text = butterflyAgent.GetCumulativeReward().ToString("0.000");
    }
}
