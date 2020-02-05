using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private struct Inputs
    {
        private float deltaX;
        private float deltaY;

        public float DeltaX { get => deltaX; set => deltaX = value; }
        public float DeltaY { get => deltaY; set => deltaY = value; }
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

    [Range(1f, 20f)]
    [SerializeField]
    private float speed = 10f;
    [Range(0f, 1f)]
    [SerializeField]
    private float boundariesOffset = 0.5f;
    [SerializeField]
    private GameObject laserPrefab;
    [Range(1f, 100f)]
    [SerializeField]
    private float projectileSpeed = 10f;
    [Range(1f,3600f)]
    [SerializeField]
    private float roundsPerMinute = 60f;
    private float reloadTime;
    [SerializeField]
    private bool needToUpgrade = true;
    private Coroutine firingCoroutine;
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
        if (Input.GetKeyDown(KeyCode.Space) && !firing)
        {
            firingCoroutine = StartCoroutine(FireContinuously());
            firing = true;
        }
        else if (Input.GetKeyUp(KeyCode.Space) && firing)
        {
            StopCoroutine(firingCoroutine);
            firing = false;
        }
    }

    IEnumerator FireContinuously()
    {
        while (true)
        {
            GameObject beam = Instantiate(laserPrefab, transform.position, Quaternion.identity) as GameObject;
            beam.GetComponent<Rigidbody2D>().velocity = Vector2.up * projectileSpeed;
            Destroy(beam, 4f);
            yield return new WaitForSeconds(reloadTime);
        }
    }

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
        Inputs i = new Inputs();
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
                deltaPos *= distance;
                i.DeltaX = deltaPos.x;
                i.DeltaY = deltaPos.y;
                return i;
            }
            else
            {
                i.DeltaX = deltaPos.x * multiplier;
                i.DeltaY = deltaPos.y * multiplier;
                return i;
            }
        }
        else
        {
            i.DeltaX = Input.GetAxis("Horizontal");
            i.DeltaY = Input.GetAxis("Vertical");
            i.DeltaX *= multiplier;
            i.DeltaY *= multiplier;
            return i;
        }
    }
}
