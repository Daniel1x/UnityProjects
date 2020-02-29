using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Config Parameters")]
    [SerializeField] [Range(0.1f, 10f)] private float playerVelocity = 1f;
    [SerializeField] [Range(0.1f, 50f)] private float jumpPower = 1f;
    [SerializeField] [Range(0.1f, 10f)] private float climbSpeed = 1f;

    //Cached components
    private Rigidbody2D rb = null;
    private CapsuleCollider2D playerCollider2D = null;
    private BoxCollider2D feetCollider2D = null;
    private Animator animator = null;
    private SpriteRenderer spriteRenderer = null;

    //Variables
    private float gravityScaleAtStart = 1f;
    private bool isAlive = true;
    private Vector3 startPosition;
    public bool IsAlive => isAlive;

    private void Start()
    {
        startPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        playerCollider2D = GetComponent<CapsuleCollider2D>();
        feetCollider2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null) Debug.LogError("There is no sprite renderer on child GameObject!");
        gravityScaleAtStart = rb.gravityScale;
    }

    private void Update()
    {
        if (!isAlive) { return; }
        Run();
        ClimbLadder();
        Jump();
        Die();
    }

    private void Run()
    {
        float speedX = playerVelocity * CrossPlatformInputManager.GetAxis("Horizontal");
        Vector2 velocity = new Vector2(speedX, rb.velocity.y);
        rb.velocity = velocity;
        
        bool playerIsRunning = Mathf.Abs(speedX) > Mathf.Epsilon;
        animator.SetBool("isRunning", playerIsRunning);

        if (playerIsRunning)
        {
            bool facingLeft = speedX > 0 ? false : true;
            spriteRenderer.flipX = facingLeft;
        }
    }

    private void ClimbLadder()
    {
        bool isTouchingLadder = feetCollider2D.IsTouchingLayers(LayerMask.GetMask(Names.LADDERS_LAYER_NAME));
        rb.gravityScale = isTouchingLadder ? 0f : gravityScaleAtStart;

        bool playerHasVerticalSpeed = Mathf.Abs(rb.velocity.y) > Mathf.Epsilon;
        animator.SetBool("isClimbing", playerHasVerticalSpeed && isTouchingLadder);

        if (!isTouchingLadder) { return; }
        Vector2 climbVelocity = new Vector2(rb.velocity.x, climbSpeed * CrossPlatformInputManager.GetAxis("Vertical"));
        rb.velocity = climbVelocity;
    }

    private void Jump()
    {
        if (!feetCollider2D.IsTouchingLayers(LayerMask.GetMask(Names.FOREGROUND_LAYER_NAME))) { return; }
        if (CrossPlatformInputManager.GetButton("Jump"))
        {
            Vector2 velocity = new Vector2(rb.velocity.x, jumpPower);
            rb.velocity = velocity;
            animator.SetTrigger("rollTrigger");
        }
    }

    private void Die()
    {
        if (playerCollider2D.IsTouchingLayers(LayerMask.GetMask(Names.ENEMIES_LAYER_NAME, Names.TRAPS_LAYER_NAME)))
        {
            isAlive = false;
            animator.SetTrigger("dieTrigger");
            rb.velocity = new Vector2(UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(10f, 50f));
            StartCoroutine(Spawn());
        }
    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(3f);
        FindObjectOfType<GameSession>().HandlePlayerDeath();
    }
}
