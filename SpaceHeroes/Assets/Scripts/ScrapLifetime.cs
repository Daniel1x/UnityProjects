using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapLifetime : MonoBehaviour
{
    public AudioClip ExplosionSound;
    public AudioSource ExplosionSoundSource;

    void Start()
    {
        ExplosionSoundSource.PlayOneShot(ExplosionSound);
        Destroy(gameObject, 20.0f);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
