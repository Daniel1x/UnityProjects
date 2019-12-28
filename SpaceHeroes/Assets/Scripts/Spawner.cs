using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{

    public GameObject WhatToSpawn;
    private float spawnRate = 0.1f;
    private float forceMultiplier = 50000f;

    void Start()
    {
        StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        GameObject gameObject = Instantiate(WhatToSpawn, transform.position, transform.rotation);
        gameObject.transform.localScale = Vector3.one * 15.0f;
        Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            //rigidbody.AddForce (transform.forward * forceMultiplier, ForceMode.Impulse);

            rigidbody.AddForce(Random.onUnitSphere * Random.Range(forceMultiplier / 3f, forceMultiplier), ForceMode.Impulse);
        }

        yield return new WaitForSeconds(1f / spawnRate);
        StartCoroutine(Spawn());
    }

}
