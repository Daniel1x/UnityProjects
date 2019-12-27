using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateMap : MonoBehaviour
{
    public float angleMultiplier = 1;
    public float xRotMax = 45;
    public float zRotMax = 45;
    Vector3 rot;

    private void Update()
    {
        transform.Rotate(new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)) * Time.deltaTime * angleMultiplier);
    }
}
