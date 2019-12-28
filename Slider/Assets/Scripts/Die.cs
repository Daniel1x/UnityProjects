using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Die : MonoBehaviour
{
    private void Start()
    {
        //Destroy(this.gameObject, 20);
    }

    private void OnBecameInvisible()
    {
        Destroy(this.gameObject, 5);
    }
}
