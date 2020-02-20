using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Health))]
public class Attacker : MonoBehaviour
{
    private float speed = 1f;
    [SerializeField] [Range(0, 250)] private int damage = 25;
    [SerializeField] [Range(0, 10)] public int baseDamage = 1;
    [SerializeField] [Range(0, 500)] public int deathIncome = 1;
    private Animator animator = null;
    private GameObject target = null;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Walk();
        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        if (!target) animator.SetBool("isAttacking", false);
    }

    private void Walk()
    {
        transform.Translate(Vector2.left * Time.deltaTime * speed);
    }

    public void SetMovementSpeed(float speed)
    {
        this.speed = speed;
    }

    public void Attack(GameObject target)
    {
        animator.SetBool("isAttacking", true);
        this.target = target;
    }

    public void AttackCurrentTarget()
    {
        if (!target) return;
        Health health = target.GetComponent<Health>();
        if (health) health.DealDamage(damage);
    }
}
