using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Base : MonoBehaviour
{
    private LivesDisplay livesDisplay = null;

    private void Start()
    {
        livesDisplay = FindObjectOfType<LivesDisplay>();
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        Attacker attacker = otherCollider.GetComponent<Attacker>();
        if (attacker) livesDisplay.TakeLifes(attacker.baseDamage);
    }
}
