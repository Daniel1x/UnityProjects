using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Coin : MonoBehaviour
{
    [SerializeField] [Range(0f, 1f)] private float volume = 0.5f;
    [SerializeField] AudioClip coinPickupSound = null;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            GameSession.activeGameSession.AddCoin(1);
            AudioSource.PlayClipAtPoint(coinPickupSound, Camera.main.transform.position, volume);
            Destroy(gameObject);
        }
    }
}
