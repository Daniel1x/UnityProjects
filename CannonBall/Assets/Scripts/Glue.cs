using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glue : MonoBehaviour
{
    private BoxCollider headCollider;
    private CapsuleCollider stickCollider;
    public GameObject arrowMesh;
    public static float power = 15f;

    private void Start()
    {
        headCollider = GetComponent<BoxCollider>();
        stickCollider = GetComponent<CapsuleCollider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Ball"))
        {
            collision.transform.GetComponent<Rigidbody>().AddForce(power * Vector3.up, ForceMode.Impulse);
            GameObject arrow = Instantiate(arrowMesh, transform.position, transform.rotation, collision.transform);
            arrow.transform.localScale = 0.25f * Vector3.one;
            Destroy(gameObject);
        }
    }
}
