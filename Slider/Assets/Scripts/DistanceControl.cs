using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DistanceControl : MonoBehaviour
{
    [SerializeField]
    Text Distance;

    [SerializeField]
    Camera Cam;

    int level;
    float size;
    float mass;
    float distance;
    float points;
    Vector3 pos;
    Vector3 camPos;

    void Start()
    {
        Distance.text = "0";
        transform.localScale = new Vector3(10f, 10f, 10f);
        size = 0;
        points = 0;
        mass = 1;
        level = 0;
    }
    
    void Update()
    {
        camPos = transform.position;
        camPos.z = camPos.z - 5 - 2*size;
        camPos.y = camPos.y + 1 + 2*size;
        Cam.transform.position = camPos;

        if (transform.position.z > 0 && transform.position.y<6)
        {
            distance = transform.position.z / 100f + (100 * level);
            if (distance - (100 * level) > points) 
            {
                Distance.text = distance.ToString("0");
                points = distance;
                if (points > 100)
                {
                    points = 0;
                    level++;
                }
            }

            size = Mathf.Clamp(10 / ((distance * distance / 100) + 1),0.1f,10f);
            mass = size * size * size;
            GetComponent<Rigidbody>().mass = mass;

            pos = transform.position;
            pos.y = (size / 2) - 0.5f;
            transform.position = pos;

            transform.localScale = new Vector3(size, size, size);

        }

        if (transform.position.z > 10025)
        {
            OutOfMap();
        }
    }

    private void OutOfMap()
    {
        transform.position = new Vector3(0, 200, -200);
    }
}
