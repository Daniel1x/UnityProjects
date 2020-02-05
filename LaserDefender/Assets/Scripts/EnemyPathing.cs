using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathing : MonoBehaviour
{
    [SerializeField] private List<Transform> waypoints = new List<Transform>();
    [Range(1f, 20f)] [SerializeField] private float enemySpeed = 1f;

    private int waypointIndex = 0;
    
    private void Update()
    {
        if (waypointIndex > waypoints.Count - 1) waypointIndex = 0;

        Vector3 targetPos = waypoints[waypointIndex].position;
        float actualMovement = enemySpeed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, targetPos, actualMovement);
        if (transform.position == targetPos) waypointIndex++;
    }
}
