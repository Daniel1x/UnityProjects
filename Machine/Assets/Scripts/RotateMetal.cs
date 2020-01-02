using UnityEngine;

public class RotateMetal : MonoBehaviour
{
    /// <summary>
    /// Rotation speed.
    /// </summary>
    [Range(0.01f,10f)] public float rotationsPerSecond = 1f;

    private void Update()
    {
        // Rotate object.
        transform.Rotate(Vector3.up, Time.deltaTime * 360f * rotationsPerSecond);
    }
}
