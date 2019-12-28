using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallStart : MonoBehaviour
{
    // config params
    [SerializeField] float pushPower = 10.0f;
    [SerializeField] List<AudioClip> audioClips = null;
    [SerializeField] float randomFactor = 0.2f;
    [Range(1f, 25f)] [SerializeField] float maxVelocity = 10.0f;

    GameObject paddle1;
    AudioSource myAudioSource = new AudioSource();
    Vector3 middleOfMap = new Vector3(8.0f, -4.0f, 0.0f);
    Vector3 offset = new Vector3();
    Rigidbody2D rb = new Rigidbody2D();
    Level level = null;
    bool hasStarted = false;

    private void Start()
    {
        paddle1 = GameObject.FindGameObjectWithTag("Player");
        offset = new Vector3(Random.Range(-2f, 2f), Random.Range(-0.5f, 1f) + 1f, 0.0f);
        rb = GetComponent<Rigidbody2D>();
        myAudioSource = GetComponent<AudioSource>();
        level = FindObjectOfType<Level>();
        level.CountPlayableBalls();
    }

    private void Update()
    {
        LockBallToPaddle();
        BlockVelocity();
    }

    private void LockBallToPaddle()
    {
        if (!hasStarted)
        {
            transform.position = paddle1.transform.position + offset;
            if (Input.GetMouseButtonDown(0))
            {
                hasStarted = true;
                rb.velocity = (transform.position - middleOfMap).normalized * pushPower;
            }
        }
    }

    private void BlockVelocity()
    {
        if (rb.velocity.magnitude > maxVelocity)
        {
            Vector2 direction = rb.velocity.normalized;
            rb.velocity = maxVelocity * direction;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ball")
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
        else
        {
            Vector2 velPush = new Vector2(UnityEngine.Random.Range(0f, randomFactor), UnityEngine.Random.Range(0f, randomFactor));
            if (hasStarted)
            {
                myAudioSource.PlayOneShot(audioClips[UnityEngine.Random.Range(0, audioClips.Capacity)]);
                rb.velocity += velPush;
            }
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject, 5.0f);
    }
}
