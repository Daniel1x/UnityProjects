using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnGround : MonoBehaviour
{
    Follow cam;

    private void Start()
    {
        cam = FindObjectOfType<Follow>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Ground"))
        {
            cam.cannonBalls.Remove(transform);
            Destroy(gameObject);
        }
    }
}
