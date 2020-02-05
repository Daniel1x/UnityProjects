using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    [SerializeField] private GameObject laserPrefab = null;
    [SerializeField] private float timer;
    [SerializeField] private float minTime = 0.5f;
    [SerializeField] private float maxTime = 2f;
    [SerializeField] private bool isShooting = false;
    [SerializeField] private float projectileSpeed = 10f;

    private IEnumerator Start()
    {
        do
        {
            yield return StartCoroutine(Reload());
        }
        while (isShooting);
    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(minTime, maxTime));
        Fire();
    }

    private void Fire()
    {
        GameObject laser = Instantiate(laserPrefab, transform.position, Quaternion.Euler(180f, 0f, 0f));
        laser.GetComponent<Rigidbody2D>().velocity = Vector2.down * projectileSpeed;
    }
}
