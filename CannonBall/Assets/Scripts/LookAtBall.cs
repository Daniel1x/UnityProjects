using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtBall : MonoBehaviour
{
    private void LateUpdate()
    {
        transform.LookAt(Follow.currentObj);
    }
}
