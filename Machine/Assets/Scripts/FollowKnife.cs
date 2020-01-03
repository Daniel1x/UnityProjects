using UnityEngine;

public class FollowKnife : MonoBehaviour
{
    /// <summary>
    /// Knife gameobject to follow.
    /// </summary>
    public GameObject knife;
    /// <summary>
    /// New position of camera.
    /// </summary>
    private Vector3 pos = new Vector3(0f, 0f, -17f);
    
    private void Start()
    {
        UpdatePosition();
    }

    private void LateUpdate()
    {
        UpdatePosition();
    }

    /// <summary>
    /// Calculate new position of camera.
    /// </summary>
    private void UpdatePosition()
    {
        pos.y = knife.transform.position.y;
        transform.position = pos;
    }
}
