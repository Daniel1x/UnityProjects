using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] [Range(1f, 10f)] private float moveSpeed = 1f;
    [SerializeField] private BoxCollider2D wallDetectorCollider2D = null;
    [SerializeField] private BoxCollider2D groundDetectorCollider2D = null;
    private Rigidbody2D rb = null;
    private SpriteRenderer spriteRenderer = null;
    private bool isFacingRight = true;
    private Vector2 wallDetectorOffset;
    private Vector2 groundDetectorOffset;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        wallDetectorOffset = wallDetectorCollider2D.offset;
        groundDetectorOffset = groundDetectorCollider2D.offset;
    }

    private void Update()
    {
        Walk();
    }

    private void Walk()
    {
        rb.velocity = new Vector2(moveSpeed * (isFacingRight ? 1 : -1), rb.velocity.y);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (wallDetectorCollider2D.IsTouchingLayers(LayerMask.GetMask(Names.FOREGROUND_LAYER_NAME))
        || !groundDetectorCollider2D.IsTouchingLayers(LayerMask.GetMask(Names.FOREGROUND_LAYER_NAME)))
        {
            RotateEnemy();
        }
    }

    private void RotateEnemy()
    {
        spriteRenderer.flipX = isFacingRight;
        isFacingRight = !isFacingRight;
        float rotationSide = isFacingRight ? 1f : -1f;
        wallDetectorCollider2D.offset = new Vector2(rotationSide * wallDetectorOffset.x, wallDetectorOffset.y);
        groundDetectorCollider2D.offset = new Vector2(rotationSide * groundDetectorOffset.x, groundDetectorOffset.y);
    }
}
