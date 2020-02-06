using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageController : MonoBehaviour
{
    private Image image;
    private float time = 0f;
    private float alpha;
    private bool started = false;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        if (started) return;
        time += Time.deltaTime;
        alpha = 0.5f + (0.5f * Mathf.Sin(((2f * time) - 0.5f) * Mathf.PI));
        image.CrossFadeAlpha(alpha, 0f, false);
        if (Input.anyKeyDown)
        {
            started = true;
            gameObject.SetActive(false);
        }
    }
}
