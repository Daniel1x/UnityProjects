using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletLifeTime : MonoBehaviour
{
    public AudioClip RocketEffect;
    public AudioSource RocketEffectSource;
    private Gun turret;
    private GameObject target;
    public float power = 100;
    
    private void Start()
    {
        Destroy(gameObject, 20.0f);
        RocketEffectSource.PlayOneShot(RocketEffect);
        turret = FindObjectOfType<Gun>();
        target = FindClosestEnemy();
    }

    private void Update()
    {
        //target = FindClosestEnemy();
        //FlyToClosest();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Body target = collision.transform.GetComponent<Body>();
        if (collision.collider.tag == "SpaceObsticle" )
        {
            target.TakeDamage(turret.GetDMG());
            Debug.Log("DMG: " + turret.GetDMG());
        }
        if (collision.collider.tag != "Bullet")
        {
            Destroy(gameObject);
        }
    }

    private GameObject FindClosestEnemy()
    {
        GameObject[] gameObjects;
        gameObjects = GameObject.FindGameObjectsWithTag("SpaceObsticle");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gameObjects)
        {
            Vector3 howFar = go.transform.position - position;
            float curDistance = howFar.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }

    private void FlyToClosest()
    {
        float distance = Vector3.Distance(target.transform.position, transform.position);
        Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
        Vector3 direction = target.transform.position - gameObject.transform.position;
        rigidbody.AddForce(Vector3.Normalize(direction) * power * Time.deltaTime * (1 / Mathf.Pow(distance, 2)));
    }
}
