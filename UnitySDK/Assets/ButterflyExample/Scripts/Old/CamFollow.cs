using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public Transform objectToFollow;

    Vector3 offset;

    private void Start()
    {
        offset = transform.position - objectToFollow.transform.position;
    }

    private void Update()
    {
        transform.position = objectToFollow.transform.position + offset;
    }

}
