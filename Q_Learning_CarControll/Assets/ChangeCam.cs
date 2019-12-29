using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCam : MonoBehaviour
{
    public GameObject[] cameras = new GameObject[5];
    private int activeCamID = 0;
    [Range(0.5f, 5f)] public float timeScale = 1f;

    private void Start()
    {
        for(int i = 0; i < cameras.Length; i++)
        {
            cameras[i].SetActive(false);
        }
        cameras[activeCamID].SetActive(true);
    }

    private void Update()
    {
        Time.timeScale = timeScale;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            cameras[activeCamID].SetActive(false);
            activeCamID++;
            activeCamID %= 5;
            cameras[activeCamID].SetActive(true);
        }
    }


}
