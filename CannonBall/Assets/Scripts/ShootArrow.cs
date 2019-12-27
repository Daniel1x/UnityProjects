using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootArrow : MonoBehaviour
{
    public GameObject arrowPrefab;
    [Range(0f, 1000f)] public float force;
    public bool isRight;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && (isRight ? (Input.mousePosition.x / Screen.width) >= 0.5f : (Input.mousePosition.x / Screen.width) <= 0.5f))
        {
            //Debug.Log(Input.mousePosition.x / Screen.width);
            GameObject arrow = Instantiate(arrowPrefab, transform.position, transform.rotation, transform.parent);
            arrow.GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.Impulse);
        }
    }
}
