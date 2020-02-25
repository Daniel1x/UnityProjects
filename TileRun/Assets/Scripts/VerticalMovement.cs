using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalMovement : MonoBehaviour
{
    [SerializeField] [Range(0f, 10f)] private float moveSpeed = 1f;

    private void Update()
    {
        Vector2 movement = Vector2.up * (moveSpeed * Time.deltaTime);
        transform.Translate(movement);
    }
}
