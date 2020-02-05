using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField] [Range(0f, 1f)] private float scrollSpeed = 1f;

    private Material material;
    private Vector2 offset;

    private void Start()
    {
        material = GetComponent<MeshRenderer>().material;
        offset = new Vector2(0f, scrollSpeed);
    }

    private void Update()
    {
        offset.y = scrollSpeed;
        material.mainTextureOffset += offset * Time.deltaTime;
    }
}
