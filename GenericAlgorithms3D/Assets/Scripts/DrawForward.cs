using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawForward : MonoBehaviour
{
    
    Vector3 look = new Vector3();

    Brain bot;
    bool started = false;

    private void Start()
    {
        started = true;
        bot = GetComponentInParent<Brain>();
    }

    private void OnDrawGizmos()
    {
        if (!started) Start();
        if (!bot.alive) return;
        look = /*transform.rotation*/ (Quaternion.Euler(68f * Vector3.right) * Vector3.forward * 10f);
        Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(0f, 0f, 0f) * look);
        Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(0f, 90f, 0f) * look);
        Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(0f, -90f, 0f) * look);
        Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(0f, 180f, 0f) * look);
        Gizmos.DrawLine(transform.position, transform.position - transform.up * 10f);
    }
    
}
