using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bush : MonoBehaviour
{
    private Attacker attacker = null;

    private void Start()
    {
        attacker = GetComponent<Attacker>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collisionGO = collision.gameObject;
        if (collisionGO.CompareTag("Defender") || collisionGO.CompareTag("Gravestone"))
        {
            attacker.Attack(collisionGO);
        }
    }
}
