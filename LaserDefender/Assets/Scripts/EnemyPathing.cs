using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathing : MonoBehaviour
{
    private List<Transform> waypoints = new List<Transform>();
    float enemySpeed = 1f;
    private int waypointIndex = 0;

    public void SetWaveConfig(WaveConfig waveConfig)
    {
        waypoints = waveConfig.GetWaypoints();
        enemySpeed = waveConfig.EnemySpeed;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (waypointIndex > waypoints.Count - 1) waypointIndex = 0;
        Vector3 targetPos = waypoints[waypointIndex].position;
        float actualMovement = enemySpeed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, targetPos, actualMovement);
        if (transform.position == targetPos) waypointIndex++;
    }
}
