using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public GameObject Bullet;
    public GameObject SpawnPoint;
    private GameObject clone;
    private readonly float speedOfBullet = 10000;
    public float fireRate = 10;
    public float timeToNextShoot = 0;

    public AudioClip SoundEffect;
    public AudioSource SoundEffectSource;

    void Update()
    {
        if (timeToNextShoot > 0)
        {
            timeToNextShoot = timeToNextShoot - Time.deltaTime;
        }
        if (Input.anyKey)
        {
            ShootBullet();
        }
    }

    public void ShootBullet()
    {
        if (timeToNextShoot <= 0)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                clone = Instantiate(Bullet, SpawnPoint.transform.position, SpawnPoint.transform.rotation);
                Rigidbody clonerb = clone.GetComponent<Rigidbody>();
                clonerb.AddForce(clone.transform.forward * speedOfBullet);
                SoundEffectSource.PlayOneShot(SoundEffect);
                timeToNextShoot = 1 / fireRate;
            }
        }
    }

}
