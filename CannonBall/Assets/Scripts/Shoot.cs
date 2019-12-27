using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public GameObject cannonBall;
    public float power;
    Follow cam;

    private void Start()
    {
        cam = FindObjectOfType<Follow>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && Input.mousePosition.y / Screen.height <= 0.3f) ShootCannonBall();
    }

    private void ShootCannonBall()
    {
        GameObject ball = Instantiate(cannonBall, new Vector3(0f, 3.6f, -0.114f), Quaternion.identity, transform);
        ball.GetComponent<Rigidbody>().AddForce(Vector3.up * power, ForceMode.Impulse);
        cam.cannonBalls.Add(ball.transform);
    }
}
