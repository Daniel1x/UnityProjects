using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    Vector3 offset = new Vector3(0f,0f,-10f);
    //public GameObject bow;
    public Transform cannon;
    public List<Transform> cannonBalls = new List<Transform>();
    public static Transform currentObj;
    public static float topPosition = 8;

    private void Start()
    {
        currentObj = cannon;
    }

    private void Update()
    {
        if (cannonBalls.Count > 0)
        {
            foreach (Transform ball in cannonBalls)
                if (ball.transform.position.y > currentObj.transform.position.y) currentObj = ball;
        }
        else currentObj = cannon;
    }

    private void LateUpdate()
    {
        if ((currentObj.transform.position + offset).y > 2.681f)
        {
            transform.position = currentObj.transform.position + offset;
        }
        else
        {
            Vector3 pos = transform.position = currentObj.transform.position + offset;
            pos.y = 2.681f;
            transform.position = pos;
        }

        /*if (transform.position.y > topPosition)
        {
            topPosition = transform.position.y;
            Vector3 pos = bow.transform.position;
            pos.y = topPosition-5f;
            bow.transform.position = pos;
        }*/
    }
}
