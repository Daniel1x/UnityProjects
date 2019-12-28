using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereColliderChange : MonoBehaviour
{
    SphereCollider collider = new SphereCollider();

    private void Start()
    {
        collider = this.GetComponent<SphereCollider>();
        collider.transform.localScale = collider.attachedRigidbody.transform.localScale;
    }

    private void Update()
    {
        collider.transform.localScale = collider.attachedRigidbody.transform.localScale;

    }
}
