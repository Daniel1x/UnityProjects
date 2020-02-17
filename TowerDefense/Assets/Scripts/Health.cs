using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float health = 100f;
    [SerializeField] private GameObject deathVFX = null;

    public void DealDamage(float damage)
    {
        health -= damage;
        CheckIfDestroyed();
    }

    private void CheckIfDestroyed()
    {
        if (health <= 0) Die();
    }

    private void Die()
    {
        TriggerDeathVFX();
        Destroy(gameObject);
    }

    private void TriggerDeathVFX()
    {
        if (!deathVFX) return;
        GameObject particle = Instantiate(deathVFX, transform.position, Quaternion.identity);
        Destroy(particle, 1f);
    }
}
