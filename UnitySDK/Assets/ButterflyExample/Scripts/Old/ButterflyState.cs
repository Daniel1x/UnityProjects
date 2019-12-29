using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButterflyState : MonoBehaviour
{
    public bool dropped = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            dropped = true;
        }
    }
}
