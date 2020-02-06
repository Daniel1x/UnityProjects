using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int health = 1000;
    [SerializeField] private GameObject explosionPrefab = null;
    [SerializeField] private AudioClip deathSound = null;
    [SerializeField] [Range(0f, 1f)] private float deathSoundVolume = 0.5f;
    [SerializeField] private Level level;
    public int Health => health;

    private void Start()
    {
        level = FindObjectOfType<Level>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DamageDealer damageDealer = collision.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer) return;
        Hit(damageDealer);
    }
    
    private void Hit(DamageDealer damageDealer)
    {
        health -= damageDealer.Damage;
        damageDealer.Hit();
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        level.LoadDelayedGameOver();
        Explode();
        Destroy(gameObject);
    }

    private void Explode()
    {
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(explosion, 1f);
        AudioSource.PlayClipAtPoint(deathSound, Camera.main.transform.position, deathSoundVolume);
    }
}
