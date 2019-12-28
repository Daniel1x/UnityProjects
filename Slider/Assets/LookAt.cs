using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    GameObject Player;

    Quaternion rotation = new Quaternion();

    void Start()
    {
        rotation = transform.rotation;
        Player = GameObject.FindGameObjectWithTag("Player");
        Destroy(gameObject, 30f);
    }

    // Update is called once per frame
    void Update()
    {
        rotation = Quaternion.LookRotation(Player.transform.position - transform.position);
        transform.rotation = rotation;
    }
}
