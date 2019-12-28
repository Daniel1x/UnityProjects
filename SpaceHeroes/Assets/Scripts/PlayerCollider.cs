using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCollider : SceneLoader
{
    public float health = 1000;
    private float timeToCollision = 0;
    private float HealTime = 0;
    private float OutOfSpaceTime = 0;
    private float OutOfSpaceDMG = 10;
    public Text PlayerHealth;
    public Text WarningField;
    private string Warning = "Warning!\nGo Back!";
    private bool WarningOn = false;

    private void Start()
    {
        PlayerHealth.text = health.ToString();
    }

    private void Update()
    {
        if (timeToCollision > 0)
        {
            timeToCollision -= Time.deltaTime;
        }
        Healing();
        OutOfSpace();
        KilledByHP();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (timeToCollision <= 0)
        {
            if (collision.collider.name == "Sun")
            {
                Debug.Log("Uwazaj na slonce!");
                health -= 250;
            }
            else if (collision.collider.name == "Black Hole")
            {
                Debug.Log("Uwazaj na czarne dziury!");
                health = 0;
            }
            else if (collision.collider.tag == "SpaceObsticle")
            {
                Debug.Log("Uwazaj na meteoryty i planety!");
                health -= 100;
            }
            else if (collision.collider.tag == "SpaceScrap")
            {
                Debug.Log("Uwazaj na kosmiczne smieci!");
                health -= 10;
            }
            if (health <= 0)
            {
                Debug.Log("Nie zyjesz!");
                LoadNextScene();
            }
            timeToCollision = 1;
            PlayerHealth.text = health.ToString();
        }
        
    }

    private void Healing()
    {
        HealTime += Time.deltaTime;
        if (HealTime >= 1)
        {
            health++;
            HealTime--;
        }
        PlayerHealth.text = health.ToString();
    }

    private void OutOfSpace()
    {
        if (Vector3.Distance(this.transform.position, Vector3.zero) > 1000)
        {
            OutOfSpaceTime += Time.deltaTime;
            if (OutOfSpaceTime >= 1)
            {
                health -= OutOfSpaceDMG;
                OutOfSpaceTime--;
                OutOfSpaceDMG++;
                WarningOn = !WarningOn;
            }
            if (WarningOn)
            {
                WarningField.text = Warning;
            }
            else
            {
                WarningField.text = "";
            }
        }
        else
        {
            WarningField.text = "";
        }
    }

    private void KilledByHP()
    {
        if (health <= 0)
        {
            Debug.Log("Nie zyjesz!");
            LoadNextScene();
        }
    }
}
