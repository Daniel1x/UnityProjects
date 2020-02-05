using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int health = 100;

    public int Health { get => health; set => health = value; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DamageDealer damageDealer = collision.gameObject.GetComponent<DamageDealer>();
        Hit(damageDealer);
    }

    private void Hit(DamageDealer damageDealer)
    {
        health -= damageDealer.Damage;
        if (health <= 0) Destroy(gameObject);
    }
}
