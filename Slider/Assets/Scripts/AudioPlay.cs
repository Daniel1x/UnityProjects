using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlay : MonoBehaviour
{
    [SerializeField]
    public AudioSource Source;
    public AudioClip Clip;


    void Start()
    {
        Source.clip = Clip;
        Source.loop = true;
        Source.Play();
    }
    
}
