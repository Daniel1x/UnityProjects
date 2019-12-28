using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowMusic : MonoBehaviour
{
    private const int numSamples = 128;

    AudioSource audio;
    private float[] samples = new float[numSamples];

    public GameObject player;
    public GameObject prefab;
    public float maxScale = 10;
    GameObject[] sampleObjects = new GameObject[numSamples];
    MeshRenderer[] meshRenderers = new MeshRenderer[numSamples];

    private void Start()
    {
        audio = GetComponent<AudioSource>();

        Vector3 distance = Vector3.right * 30;
        for(int i = 0; i < numSamples; i++)
        {
            GameObject instanceObject = Instantiate(prefab);
            instanceObject.transform.position = player.transform.position + Quaternion.Euler(0,0,i*180f/numSamples) * distance;
            instanceObject.transform.parent = player.transform;
            instanceObject.transform.LookAt(player.transform);
            instanceObject.name = "Spike" + i;
            sampleObjects[i] = instanceObject;
            meshRenderers[i] = instanceObject.GetComponent<MeshRenderer>();
        }
    }

    private void Update()
    {
        audio.GetSpectrumData(samples, 0, FFTWindow.Blackman);

        for(int i = 0; i < numSamples/2; i++)
        {
            float val = samples[i] * 10f;
            sampleObjects[i].transform.localScale = new Vector3(0.1f, 0.1f, val);
            meshRenderers[i].material.color = new Color(val, 0, 0);
            sampleObjects[numSamples - i - 1].transform.localScale = new Vector3(0.1f, 0.1f, val);
            meshRenderers[numSamples - i - 1].GetComponent<MeshRenderer>().material.color = new Color(val, 0, 0);
        }
    }
}
