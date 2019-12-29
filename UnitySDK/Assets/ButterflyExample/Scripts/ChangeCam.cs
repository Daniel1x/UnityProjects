using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCam : MonoBehaviour
{
    public int nObj = 4;
    public GameObject[] objectsToFollow = new GameObject[4];
    GameObject currentObj;
    Vector3 offset;
    [Range(0.5f, 5f)] public float timeScale = 1f;
    int id = 1;

    private void Start()
    {
        offset = transform.position - objectsToFollow[0].transform.position;
        currentObj = objectsToFollow[0];
    }
    private void Update()
    {
        Time.timeScale = timeScale;
        id %= nObj;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentObj = objectsToFollow[id++];
        }
    }
    private void LateUpdate()
    {
        transform.position = currentObj.transform.position + offset;
    }
}
