using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusScript : MonoBehaviour
{
    private float delay = 0;
    private Gun turret;

    private void Start()
    {
        turret = FindObjectOfType<Gun>();
    }

    private void Update()
    {
        delay += Time.deltaTime;
        if (delay > 0.5f)
        {
            GetComponent<Collider>().isTrigger = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.collider.tag);
        if (collision.collider.CompareTag("Player"))
        {
            Destroy(gameObject);
            turret.ItIsTimeToUpgrade();
        }
    }
}
