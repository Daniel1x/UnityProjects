using UnityEngine;

public class FollowKnife : MonoBehaviour
{
    public GameObject knife;
    public float zAxisOffset = -10f;
    private Vector3 pos = new Vector3(0f, 0f, -10f);

    private void Start()
    {
        pos.z = zAxisOffset;
        pos.y = knife.transform.position.y;
        transform.position = pos;
    }

    private void LateUpdate()
    {
        pos.y = knife.transform.position.y;
        transform.position = pos;
    }
}
