using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] [Range(-360f, 360f)] private float rotationSpeed = -180f;
    [SerializeField] [Range(0f, 10f)] private float projectileSpeed = 1f;
    [SerializeField] private float damage = 50f;

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        float time = Time.deltaTime;
        transform.Translate(Vector2.right * projectileSpeed * time, transform.parent);
        transform.Rotate(Vector3.forward * rotationSpeed * time);
    }

    private void OnTriggerEnter2D(Collider2D enemy)
    {
        if (!enemy.CompareTag("Attacker")) return;
        Health health = enemy.GetComponent<Health>();
        if (health)
        {
            health.DealDamage(damage);
            Destroy(gameObject);
        }
    }
}
