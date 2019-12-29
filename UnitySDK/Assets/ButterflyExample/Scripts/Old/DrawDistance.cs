using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawDistance : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 10f);
    }
}
