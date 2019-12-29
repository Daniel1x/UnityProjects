using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowRays : MonoBehaviour
{
    public bool show = false;

    private void OnDrawGizmos()
    {
        if (!show) return;
        Gizmos.DrawLine(transform.position, transform.position + transform.rotation * (Vector3.forward * 100f));
        Gizmos.DrawLine(transform.position, transform.position + transform.rotation * (Quaternion.Euler(0f, 45f, 0f) * Vector3.forward * 100f));
        Gizmos.DrawLine(transform.position, transform.position + transform.rotation * (Quaternion.Euler(0f, 90f, 0f) * Vector3.forward * 100f));
        Gizmos.DrawLine(transform.position, transform.position + transform.rotation * (Quaternion.Euler(0f, -45f, 0f) * Vector3.forward * 100f));
        Gizmos.DrawLine(transform.position, transform.position + transform.rotation * (Quaternion.Euler(0f, -90f, 0f) * Vector3.forward * 100f));
    }
}
