using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Attacker))]
public class Fox : MonoBehaviour
{
    private Attacker attacker = null;
    private Animator animator = null;

    private void Start()
    {
        attacker = GetComponent<Attacker>();
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collisionGO = collision.gameObject;
        if (collisionGO.CompareTag("Defender"))
        {
            attacker.Attack(collisionGO);
        }
        else if (collisionGO.CompareTag("Gravestone"))
        {
            animator.SetTrigger("jumpTrigger");
        }
    }
}
