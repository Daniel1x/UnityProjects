using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using TMPro;
using System;

public class ButterflyAreaToTarget : Area
{
    public ButterflyAgentToTarget butterflyAgent;
    public GameObject targetPrefab;
    public TextMeshPro cumulativeRewardText;

    public float targetDistance = 10f;
    public float catchRange = 1f;
    public float butterflySpawnRange = 5f;
    public float targetSpawnRange = 10f;

    public List<GameObject> targetList;

    public override void ResetArea()
    {
        RemoveAllTargets();
        SpawnButterfly();
        SpawnTarget(targetDistance);
    }

    public void RemoveSpecificTarget(GameObject targetObject)
    {
        targetList.Remove(targetObject);
        Destroy(targetObject);
    }

    public void MoveSpecificTarget(GameObject targetObject)
    {
        targetObject.transform.position = ChooseRandomPosition(transform.position + new Vector3(butterflyAgent.transform.position.x,
                                                                0f, butterflyAgent.transform.position.z), 0f, 360f, targetDistance, targetDistance, true);
        targetObject.transform.rotation = Quaternion.Euler(UnityEngine.Random.Range(0f, 360), UnityEngine.Random.Range(0f, 360), UnityEngine.Random.Range(0f, 360));
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

    private void SpawnTarget(float targetDistance)
    {
        GameObject targetObject = Instantiate(targetPrefab);
        targetObject.transform.position = ChooseRandomPosition(transform.position + new Vector3(butterflyAgent.transform.position.x, 
                                                                0f, butterflyAgent.transform.position.z), 0f, 360f, targetDistance, targetDistance, true);
        targetObject.transform.rotation = Quaternion.Euler(UnityEngine.Random.Range(0f, 360), UnityEngine.Random.Range(0f, 360), UnityEngine.Random.Range(0f, 360));
        targetObject.transform.parent = transform;
        targetList.Add(targetObject);
        targetObject.GetComponent<Target>().targetSpeed = 0f;
    }

    private void Update()
    {
        cumulativeRewardText.text = butterflyAgent.GetCumulativeReward().ToString("0.000");
    }
}
