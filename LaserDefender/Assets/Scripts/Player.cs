using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private class Inputs
    {
        private float deltaX;
        private float deltaY;

        public float DeltaX { get => deltaX; set => deltaX = value; }
        public float DeltaY { get => deltaY; set => deltaY = value; }

        public Inputs(float dX = 0f, float dY = 0f)
        {
            deltaX = dX;
            deltaY = dY;
        }

        public Inputs(Vector3 v)
        {
            deltaX = v.x;
            deltaY = v.y;
        }

        public static Inputs operator *(float val, Inputs inputs)
        {
            return new Inputs(val * inputs.deltaX, val * inputs.deltaY);
        }

        public static Inputs operator *(Inputs inputs, float val)
        {
            return new Inputs(val * inputs.deltaX, val * inputs.deltaY);
        }

        public static implicit operator Inputs(Vector3 v)
        {
            return new Inputs(v);
        }
    }
    private struct Boundaries
    {
        float min;
        float max;

        public float Min { get => min; set => min = value; }
        public float Max { get => max; set => max = value; }
    }
    private Inputs inputs;
    private Boundaries XBoundaries;
    private Boundaries YBoundaries;
    
    [SerializeField] private float speed = 10f;
    [SerializeField] private float boundariesOffset = 0.5f;
    [SerializeField] private GameObject laserPrefab = null;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float roundsPerMinute = 60f;
    [SerializeField] private bool needToUpgrade = true;
    //[SerializeField] private AudioClip[] soundEffects = null;
    //[SerializeField] private bool changeSoundEffects = false;
    [SerializeField] private AudioClip shootSound = null;
    [SerializeField] [Range(0f, 1f)] private float shootVolume = 0.5f;

    private Coroutine firingCoroutine;
    private float reloadTime;
    private bool firing = false;

    private void Start()
    {
        SetUpMoveBoundaries();
    }

    private void SetUpMoveBoundaries()
    {
        Camera cam = Camera.main;
        XBoundaries.Min = cam.ViewportToWorldPoint(Vector3.zero).x + boundariesOffset;
        XBoundaries.Max = cam.ViewportToWorldPoint(Vector3.right).x - boundariesOffset;
        YBoundaries.Min = cam.ViewportToWorldPoint(Vector3.zero).y + boundariesOffset;
        YBoundaries.Max = cam.ViewportToWorldPoint(Vector3.up).y - boundariesOffset;
    }

    private void Update()
    {
        UpgradeShip();
        Move();
        Fire();
    }

    private void UpgradeShip()
    {
        if (!needToUpgrade) return;
        needToUpgrade = false;
        reloadTime = 60f / roundsPerMinute;
    }

    private void Fire()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && !firing)
        {
            firingCoroutine = StartCoroutine(FireContinuously());
            firing = true;
        }
        else if ((Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(0)) && firing)
        {
            StopCoroutine(firingCoroutine);
            firing = false;
        }
    }

    IEnumerator FireContinuously()
    {
        while (true)
        {
            Shoot();
            yield return new WaitForSeconds(reloadTime);
        }
    }

    private void Shoot()
    {
        GameObject laser = Instantiate(laserPrefab, transform.position, Quaternion.identity) as GameObject;
        laser.GetComponent<Rigidbody2D>().velocity = Vector2.up * projectileSpeed;
        Destroy(laser, 5f);
        //LoadNextSountEffect();
        AudioSource.PlayClipAtPoint(shootSound, Camera.main.transform.position, shootVolume);
    }

    /*private int soundID = -1;
    private void LoadNextSountEffect()
    {
        if (!changeSoundEffects) return;
        soundID++;
        if (soundID > soundEffects.Length - 1) soundID = 0;
        shootSound = soundEffects[soundID];
    }*/

    private void Move()
    {
        Inputs inputs = GetInputs();
        Vector2 newPos = transform.position + new Vector3(inputs.DeltaX, inputs.DeltaY, 0f);
        newPos.x = Mathf.Clamp(newPos.x, XBoundaries.Min, XBoundaries.Max);
        newPos.y = Mathf.Clamp(newPos.y, YBoundaries.Min, YBoundaries.Max);
        transform.position = newPos;
    }

    Inputs GetInputs()
    {
        Inputs inputs = new Inputs();
        float multiplier = Time.deltaTime * speed;
        if (Input.GetMouseButton(0))
        {
            Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 deltaPos = (clickPos - transform.position);
            deltaPos.z = 0f;
            float distance = deltaPos.magnitude;
            deltaPos.Normalize();
            if (distance < (deltaPos.magnitude * multiplier))
            {
                inputs = deltaPos * distance;
                return inputs;
            }
            else
            {
                inputs = deltaPos * multiplier;
                return inputs;
            }
        }
        else
        {
            inputs.DeltaX = Input.GetAxis("Horizontal");
            inputs.DeltaY = Input.GetAxis("Vertical");
            inputs *= multiplier;
            return inputs;
        }
    }
}
