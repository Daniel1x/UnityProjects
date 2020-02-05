using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserShredder : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Laser") || collision.CompareTag("PlayerLaser") || collision.CompareTag("EnemyLaser") )
        {
            Destroy(collision.gameObject);
        }
    }
}
