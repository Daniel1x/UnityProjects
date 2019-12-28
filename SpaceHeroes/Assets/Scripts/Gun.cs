using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Gun : MonoBehaviour {

    private float damage = 20.0f;
    private readonly float range = 500.0f;
    //private readonly float impForce = 1000000.0f;
    private float rateofFire = 1.0f;
    private float nextTimeToFire = 0f;
    public Text healthText;
    public Text damageText;
    public Text timer;
    public Text fireRate;
    public Text ammoRack;
    public Text ammoLoadBox;
    public Camera ShipCam;
    public ParticleSystem GunFlash;
    public GameObject impactEffect;
    private float timeToUpgrade = 60.0f;
    private float UpgradeTime = 30.0f;

    //shoot
    public GameObject Bullet;
    public GameObject SpawnPoint;
    private GameObject BulletClone;
    private readonly float speedOfBullet = 20000;
    public float timeToNextShoot = 0;
    private float increaseROF = 0.25f;
    private float increaseDMG = 2.5f;
    public AudioClip SoundEffect;
    public AudioSource SoundEffectSource;
    private int Ammo = 50;
    private float timeToAmmo = 1;
    private int ammoLoad = 1;
    private float timeToAmmoUpgrade = 60;
    //shoot

    private void Start()
    {
        ShowDMG();
        ShowROF();
        ShowAmmoLoad();
    }

    void Update()
    {
        if (timeToNextShoot > 0)
        {
            timeToNextShoot = timeToNextShoot - Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Space) && Time.time >= nextTimeToFire) 
        {
            if (Ammo > 0)
            {
                nextTimeToFire = Time.time + 1f / rateofFire;
                Shoot();
                ShootBullet();
                Ammo--;
                ammoRack.text = Ammo.ToString();
            }
        }
        CountTime();
        TimeToUpgrade();
    }

    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(ShipCam.transform.position, ShipCam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);
            Body target = hit.transform.GetComponent<Body>();
            if (target != null)
            {
                //target.TakeDamage(damage);
                ShowHealthPoints(target);
            }
            if (hit.rigidbody)
            {
                //hit.rigidbody.AddForce(-hit.normal * impForce);
            }
            GameObject impGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impGO, 2.0f);
        }
    }

    private void ShowHealthPoints(Body target)
    {
        Debug.Log(target.health);
        if (target.health > 0)
        {
            healthText.text = target.health.ToString();
        }
        else
        {
            healthText.text = 0.ToString();
        }
    }

    private void ShowDMG()
    {
        damageText.text = damage.ToString();
    }

    private void IncreaseDMG()
    {
        damage += increaseDMG;
    }

    private void IncreaseAmmoLoad()
    {
        if (ammoLoad < 50)
        {
            ammoLoad++;
        }
    }

    private void ShowAmmoLoad()
    {
        ammoLoadBox.text = ammoLoad.ToString();
    }

    private void IncreaseROF()
    {
        if (rateofFire < 49)
        {
            rateofFire += increaseROF;
        }
        else
        {
            rateofFire = 50;
        }
    }

    private void ShowROF()
    {
        fireRate.text = rateofFire.ToString();
    }

    private void TimeToUpgrade()
    {
        if (timeToUpgrade <= 0)
        {
            IncreaseDMG();
            ShowDMG();
            IncreaseROF();
            ShowROF();
            IncreaseAmmoLoad();
            ShowAmmoLoad();
            timeToUpgrade = UpgradeTime;
        }
    }

    private void CountTime()
    {
        timeToUpgrade -= Time.deltaTime;
        timeToAmmo -= Time.deltaTime;
        timeToAmmoUpgrade -= Time.deltaTime;
        if (timeToUpgrade >= 0)
        {
            timer.text = timeToUpgrade.ToString();
        }
        if (timeToAmmo <= 0)
        {
            Ammo+=ammoLoad;
            timeToAmmo = 10;
            ammoRack.text = Ammo.ToString();
        }
        if (timeToAmmoUpgrade <= 0)
        {
            if (ammoLoad < 50)
            {
                ammoLoad++;
            }
            timeToAmmoUpgrade = 60;
        }
    }

    public float GetDMG()
    {
        return damage;
    }
    
    public void ShootBullet()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            BulletClone = Instantiate(Bullet, SpawnPoint.transform.position, SpawnPoint.transform.rotation);
            Rigidbody clonerb = BulletClone.GetComponent<Rigidbody>();
            clonerb.AddForce(BulletClone.transform.forward * speedOfBullet);
            SoundEffectSource.PlayOneShot(SoundEffect);
        }
    }
    
    public void ItIsTimeToUpgrade()
    {
        timeToUpgrade = 0;
    }
}
