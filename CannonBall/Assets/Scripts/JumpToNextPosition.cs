using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpToNextPosition : MonoBehaviour
{
    Transform cam;

    private void Start()
    {
        cam = Camera.main.transform;
    }
    
    private void Update()
    {
        float diff = transform.position.y - cam.position.y;
        Vector3 pos = transform.position;
        if (diff > 20f)
        {
            pos.y -= 30f;
            transform.position = pos;
        }else if (diff < -20f)
        {
            pos.y += 30f;
            transform.position = pos;
        }
    }
}
